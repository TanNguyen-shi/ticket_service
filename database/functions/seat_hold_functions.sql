-- =========================================================
-- MODULE: SEAT_HOLD
-- TABLE : ticketing.seat_hold
-- NOTE  : refcursor, only write/read data
-- =========================================================

-- DROP FUNCTION IF EXISTS ticketing.seat_hold_check(bigint, varchar);
-- DROP FUNCTION IF EXISTS ticketing.seat_hold_insert(varchar, bigint, bigint, varchar, timestamp without time zone, timestamp without time zone, timestamp without time zone, varchar);
-- DROP FUNCTION IF EXISTS ticketing.seat_hold_update(bigint, varchar, bigint, bigint, varchar, timestamp without time zone, timestamp without time zone, timestamp without time zone, varchar);
-- DROP FUNCTION IF EXISTS ticketing.seat_hold_delete(bigint);
-- DROP FUNCTION IF EXISTS ticketing.seat_hold_getbyid(bigint);
-- DROP FUNCTION IF EXISTS ticketing.seat_hold_getpagedlist(integer, integer, varchar, bigint, bigint, varchar);



CREATE OR REPLACE FUNCTION ticketing.seat_hold_check(
    p_hold_id bigint DEFAULT 0,
    p_hold_code varchar DEFAULT ''
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
DECLARE
    v_out refcursor := 'seat_hold_check';
    v_exists integer := 0;
BEGIN
    SELECT CASE
        WHEN EXISTS
        (
            SELECT 1
            FROM ticketing.seat_hold h
            WHERE h.hold_code = trim(p_hold_code)
              AND (COALESCE(p_hold_id, 0) = 0 OR h.hold_id <> p_hold_id)
        )
        THEN 1
        ELSE 0
    END
    INTO v_exists;

    OPEN v_out FOR
    SELECT v_exists AS is_exists;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_insert(
    p_hold_code varchar,
    p_event_id bigint,
    p_customer_id bigint,
    p_status varchar,
    p_hold_started_at timestamp without time zone,
    p_hold_expires_at timestamp without time zone,
    p_released_at timestamp without time zone,
    p_release_reason varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_insert(
        p_hold_code => 'HOLD0001',
        p_event_id => 5,
        p_customer_id => 1,
        p_status => 'active',
        p_hold_started_at => now()::timestamp,
        p_hold_expires_at => (now() + interval '10 minute')::timestamp,
        p_released_at => NULL,
        p_release_reason => NULL
    );
    FETCH ALL IN "seat_hold_insert";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_insert';
    v_hold_id bigint;
BEGIN
    INSERT INTO ticketing.seat_hold
    (
        hold_code,
        event_id,
        customer_id,
        status,
        hold_started_at,
        hold_expires_at,
        released_at,
        release_reason,
        created_at
    )
    VALUES
    (
        trim(p_hold_code),
        p_event_id,
        p_customer_id,
        p_status,
        p_hold_started_at,
        p_hold_expires_at,
        p_released_at,
        p_release_reason,
        now()
    )
    RETURNING hold_id INTO v_hold_id;

    OPEN v_out FOR
    SELECT v_hold_id AS hold_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_update(
    p_hold_id bigint,
    p_hold_code varchar,
    p_event_id bigint,
    p_customer_id bigint,
    p_status varchar,
    p_hold_started_at timestamp without time zone,
    p_hold_expires_at timestamp without time zone,
    p_released_at timestamp without time zone,
    p_release_reason varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_update(
        p_hold_id => 1,
        p_hold_code => 'HOLD0001',
        p_event_id => 5,
        p_customer_id => 1,
        p_status => 'released',
        p_hold_started_at => '2026-01-01 10:00:00'::timestamp,
        p_hold_expires_at => '2026-01-01 10:10:00'::timestamp,
        p_released_at => '2026-01-01 10:05:00'::timestamp,
        p_release_reason => 'manual_release'
    );
    FETCH ALL IN "seat_hold_update";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_update';
BEGIN
    UPDATE ticketing.seat_hold
    SET
        hold_code = trim(p_hold_code),
        event_id = p_event_id,
        customer_id = p_customer_id,
        status = p_status,
        hold_started_at = p_hold_started_at,
        hold_expires_at = p_hold_expires_at,
        released_at = p_released_at,
        release_reason = p_release_reason
    WHERE hold_id = p_hold_id;

    OPEN v_out FOR
    SELECT p_hold_id AS hold_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_delete(
    p_hold_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_delete(1);
    FETCH ALL IN "seat_hold_delete";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_delete';
BEGIN
    DELETE FROM ticketing.seat_hold
    WHERE hold_id = p_hold_id;

    OPEN v_out FOR
    SELECT p_hold_id AS hold_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_getbyid(
    p_hold_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_getbyid(1);
    FETCH ALL IN "seat_hold_getbyid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_getbyid';
BEGIN
    OPEN v_out FOR
    SELECT
        h.hold_id,
        h.hold_code,
        h.event_id,
        e.event_code,
        e.event_name,
        h.customer_id,
        c.email,
        c.full_name,
        h.status,
        h.hold_started_at,
        h.hold_expires_at,
        h.released_at,
        h.release_reason,
        h.created_at
    FROM ticketing.seat_hold h
    LEFT JOIN ticketing."event" e
        ON e.event_id = h.event_id
    LEFT JOIN ticketing.customer c
        ON c.customer_id = h.customer_id
    WHERE h.hold_id = p_hold_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_getpagedlist(
    p_pagesize integer,
    p_offset integer,
    p_keysearch varchar,
    p_event_id bigint,
    p_customer_id bigint,
    p_status varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_event_id => -1,
        p_customer_id => -1,
        p_status => ''
    );
    FETCH ALL IN "seat_hold_getpagedlist";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_getpagedlist';
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY h.created_at DESC, h.hold_id DESC
            ) AS row_index,
            COUNT(*) OVER () AS row_total,

            h.hold_id,
            h.hold_code,
            h.event_id,
            e.event_code,
            e.event_name,
            h.customer_id,
            c.email,
            c.full_name,
            h.status,
            h.hold_started_at,
            h.hold_expires_at,
            h.released_at,
            h.release_reason,
            h.created_at
        FROM ticketing.seat_hold h
        LEFT JOIN ticketing."event" e
            ON e.event_id = h.event_id
        LEFT JOIN ticketing.customer c
            ON c.customer_id = h.customer_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(h.hold_code) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.email, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_event_id = -1
                OR h.event_id = p_event_id
              )
          AND (
                p_customer_id = -1
                OR h.customer_id = p_customer_id
              )
          AND (
                trim(coalesce(p_status, '')) = ''
                OR h.status = p_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$function$;


-- =========================================================
-- ADDITIONAL FUNCTIONS FOR SEATHOLD MODULE
-- =========================================================

CREATE OR REPLACE FUNCTION ticketing.seat_hold_getbyeventid(
    p_event_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_getbyeventid(5);
    FETCH ALL IN "seat_hold_getbyeventid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_getbyeventid';
BEGIN
    OPEN v_out FOR
    SELECT
        h.hold_id,
        h.hold_code,
        h.event_id,
        e.event_code,
        e.event_name,
        h.customer_id,
        c.email,
        c.full_name,
        h.status,
        h.hold_started_at,
        h.hold_expires_at,
        h.released_at,
        h.release_reason,
        h.created_at
    FROM ticketing.seat_hold h
    LEFT JOIN ticketing."event" e
        ON e.event_id = h.event_id
    LEFT JOIN ticketing.customer c
        ON c.customer_id = h.customer_id
    WHERE h.event_id = p_event_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_getbycustomerid(
    p_customer_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_getbycustomerid(1);
    FETCH ALL IN "seat_hold_getbycustomerid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_getbycustomerid';
BEGIN
    OPEN v_out FOR
    SELECT
        h.hold_id,
        h.hold_code,
        h.event_id,
        e.event_code,
        e.event_name,
        h.customer_id,
        c.email,
        c.full_name,
        h.status,
        h.hold_started_at,
        h.hold_expires_at,
        h.released_at,
        h.release_reason,
        h.created_at
    FROM ticketing.seat_hold h
    LEFT JOIN ticketing."event" e
        ON e.event_id = h.event_id
    LEFT JOIN ticketing.customer c
        ON c.customer_id = h.customer_id
    WHERE h.customer_id = p_customer_id;

    RETURN v_out;
END;
$function$;



-- =========================================================
-- MODULE: SEAT_HOLD_ITEM
-- TABLE : ticketing.seat_hold_item
-- NOTE  : refcursor, only write/read data
-- =========================================================

-- DROP FUNCTION IF EXISTS ticketing.seat_hold_item_check(bigint, bigint, bigint);
-- DROP FUNCTION IF EXISTS ticketing.seat_hold_item_insert(bigint, bigint, bigint, bigint, numeric, varchar, varchar, varchar);
-- DROP FUNCTION IF EXISTS ticketing.seat_hold_item_update(bigint, bigint, bigint, bigint, bigint, numeric, varchar, varchar, varchar);
-- DROP FUNCTION IF EXISTS ticketing.seat_hold_item_delete(bigint);
-- DROP FUNCTION IF EXISTS ticketing.seat_hold_item_getbyid(bigint);
-- DROP FUNCTION IF EXISTS ticketing.seat_hold_item_getpagedlist(integer, integer, bigint, bigint, bigint, varchar);



CREATE OR REPLACE FUNCTION ticketing.seat_hold_item_check(
    p_hold_item_id bigint DEFAULT 0,
    p_hold_id bigint DEFAULT 0,
    p_event_seat_inventory_id bigint DEFAULT 0
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
DECLARE
    v_out refcursor := 'seat_hold_item_check';
    v_exists integer := 0;
BEGIN
    SELECT CASE
        WHEN EXISTS
        (
            SELECT 1
            FROM ticketing.seat_hold_item hi
            WHERE hi.hold_id = p_hold_id
              AND hi.event_seat_inventory_id = p_event_seat_inventory_id
              AND (COALESCE(p_hold_item_id, 0) = 0 OR hi.hold_item_id <> p_hold_item_id)
        )
        THEN 1
        ELSE 0
    END
    INTO v_exists;

    OPEN v_out FOR
    SELECT v_exists AS is_exists;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_item_insert(
    p_hold_id bigint,
    p_event_seat_inventory_id bigint,
    p_seat_id bigint,
    p_zone_id bigint,
    p_price_at_hold numeric,
    p_seat_label_snapshot varchar,
    p_zone_name_snapshot varchar,
    p_status varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_item_insert(
        p_hold_id => 1,
        p_event_seat_inventory_id => 1001,
        p_seat_id => 16101,
        p_zone_id => 17,
        p_price_at_hold => 3500000,
        p_seat_label_snapshot => 'A01',
        p_zone_name_snapshot => 'Khu VIP',
        p_status => 'active'
    );
    FETCH ALL IN "seat_hold_item_insert";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_insert';
    v_hold_item_id bigint;
BEGIN
    INSERT INTO ticketing.seat_hold_item
    (
        hold_id,
        event_seat_inventory_id,
        seat_id,
        zone_id,
        price_at_hold,
        seat_label_snapshot,
        zone_name_snapshot,
        status,
        created_at
    )
    VALUES
    (
        p_hold_id,
        p_event_seat_inventory_id,
        p_seat_id,
        p_zone_id,
        p_price_at_hold,
        p_seat_label_snapshot,
        p_zone_name_snapshot,
        p_status,
        now()
    )
    RETURNING hold_item_id INTO v_hold_item_id;

    OPEN v_out FOR
    SELECT v_hold_item_id AS hold_item_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_item_update(
    p_hold_item_id bigint,
    p_hold_id bigint,
    p_event_seat_inventory_id bigint,
    p_seat_id bigint,
    p_zone_id bigint,
    p_price_at_hold numeric,
    p_seat_label_snapshot varchar,
    p_zone_name_snapshot varchar,
    p_status varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_item_update(
        p_hold_item_id => 1,
        p_hold_id => 1,
        p_event_seat_inventory_id => 1001,
        p_seat_id => 16101,
        p_zone_id => 17,
        p_price_at_hold => 3500000,
        p_seat_label_snapshot => 'A01',
        p_zone_name_snapshot => 'Khu VIP',
        p_status => 'converted'
    );
    FETCH ALL IN "seat_hold_item_update";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_update';
BEGIN
    UPDATE ticketing.seat_hold_item
    SET
        hold_id = p_hold_id,
        event_seat_inventory_id = p_event_seat_inventory_id,
        seat_id = p_seat_id,
        zone_id = p_zone_id,
        price_at_hold = p_price_at_hold,
        seat_label_snapshot = p_seat_label_snapshot,
        zone_name_snapshot = p_zone_name_snapshot,
        status = p_status
    WHERE hold_item_id = p_hold_item_id;

    OPEN v_out FOR
    SELECT p_hold_item_id AS hold_item_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_item_delete(
    p_hold_item_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_item_delete(1);
    FETCH ALL IN "seat_hold_item_delete";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_delete';
BEGIN
    DELETE FROM ticketing.seat_hold_item
    WHERE hold_item_id = p_hold_item_id;

    OPEN v_out FOR
    SELECT p_hold_item_id AS hold_item_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_item_getbyid(
    p_hold_item_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_item_getbyid(1);
    FETCH ALL IN "seat_hold_item_getbyid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_getbyid';
BEGIN
    OPEN v_out FOR
    SELECT
        hi.hold_item_id,
        hi.hold_id,
        h.hold_code,
        h.event_id,
        e.event_code,
        e.event_name,

        hi.event_seat_inventory_id,
        hi.seat_id,
        s.seat_code,
        s.row_label,
        s.seat_number,
        s.seat_label,

        hi.zone_id,
        ez.zone_code,
        ez.zone_name,

        hi.price_at_hold,
        hi.seat_label_snapshot,
        hi.zone_name_snapshot,
        hi.status,
        hi.created_at
    FROM ticketing.seat_hold_item hi
    LEFT JOIN ticketing.seat_hold h
        ON h.hold_id = hi.hold_id
    LEFT JOIN ticketing."event" e
        ON e.event_id = h.event_id
    LEFT JOIN ticketing.venue_seat s
        ON s.seat_id = hi.seat_id
    LEFT JOIN ticketing.event_zone ez
        ON ez.event_zone_id = hi.zone_id
    WHERE hi.hold_item_id = p_hold_item_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_item_getpagedlist(
    p_pagesize integer,
    p_offset integer,
    p_hold_id bigint,
    p_event_seat_inventory_id bigint,
    p_zone_id bigint,
    p_status varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_item_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_hold_id => -1,
        p_event_seat_inventory_id => -1,
        p_zone_id => -1,
        p_status => ''
    );
    FETCH ALL IN "seat_hold_item_getpagedlist";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_getpagedlist';
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY hi.created_at DESC, hi.hold_item_id DESC
            ) AS row_index,
            COUNT(*) OVER () AS row_total,

            hi.hold_item_id,
            hi.hold_id,
            h.hold_code,
            h.event_id,
            e.event_code,
            e.event_name,

            hi.event_seat_inventory_id,
            hi.seat_id,
            s.seat_code,
            s.row_label,
            s.seat_number,
            s.seat_label,

            hi.zone_id,
            ez.zone_code,
            ez.zone_name,

            hi.price_at_hold,
            hi.seat_label_snapshot,
            hi.zone_name_snapshot,
            hi.status,
            hi.created_at
        FROM ticketing.seat_hold_item hi
        LEFT JOIN ticketing.seat_hold h
            ON h.hold_id = hi.hold_id
        LEFT JOIN ticketing."event" e
            ON e.event_id = h.event_id
        LEFT JOIN ticketing.venue_seat s
            ON s.seat_id = hi.seat_id
        LEFT JOIN ticketing.event_zone ez
            ON ez.event_zone_id = hi.zone_id
        WHERE (
                p_hold_id = -1
                OR hi.hold_id = p_hold_id
              )
          AND (
                p_event_seat_inventory_id = -1
                OR hi.event_seat_inventory_id = p_event_seat_inventory_id
              )
          AND (
                p_zone_id = -1
                OR hi.zone_id = p_zone_id
              )
          AND (
                trim(coalesce(p_status, '')) = ''
                OR hi.status = p_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.seat_hold_item_getbyholdid(
    p_hold_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.seat_hold_item_getbyholdid(1);
    FETCH ALL IN "seat_hold_item_getbyholdid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_getbyholdid';
BEGIN
    OPEN v_out FOR
    SELECT
        hi.hold_item_id,
        hi.hold_id,
        h.hold_code,
        h.event_id,
        e.event_code,
        e.event_name,

        hi.event_seat_inventory_id,
        hi.seat_id,
        s.seat_code,
        s.row_label,
        s.seat_number,
        s.seat_label,

        hi.zone_id,
        ez.zone_code,
        ez.zone_name,

        hi.price_at_hold,
        hi.seat_label_snapshot,
        hi.zone_name_snapshot,
        hi.status,
        hi.created_at
    FROM ticketing.seat_hold_item hi
    LEFT JOIN ticketing.seat_hold h
        ON h.hold_id = hi.hold_id
    LEFT JOIN ticketing."event" e
        ON e.event_id = h.event_id
    LEFT JOIN ticketing.venue_seat s
        ON s.seat_id = hi.seat_id
    LEFT JOIN ticketing.event_zone ez
        ON ez.event_zone_id = hi.zone_id
    WHERE hi.hold_id = p_hold_id;

    RETURN v_out;
END;
$function$;

