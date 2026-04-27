-- =========================================================
-- MIGRATION: Add customer_id to seat_hold
-- PURPOSE  : Support client bookings via customer_id (separate from admin user_id)
-- =========================================================

-- Step 1: Add customer_id column to seat_hold, make user_id nullable
-- ALTER TABLE ticketing.seat_hold
--     ADD COLUMN IF NOT EXISTS customer_id BIGINT REFERENCES ticketing.customer(customer_id),
--     ALTER COLUMN user_id DROP NOT NULL;

-- Step 2: Add index on (customer_id, event_id, status) for client booking lookups
-- CREATE INDEX IF NOT EXISTS idx_seat_hold_customer_event_status
--     ON ticketing.seat_hold(customer_id, event_id, status)
--     WHERE customer_id IS NOT NULL;



-- =========================================================
-- Updated seat_hold_insert: now accepts p_customer_id
-- =========================================================

CREATE OR REPLACE FUNCTION ticketing.seat_hold_insert(
    p_hold_code varchar,
    p_event_id bigint,
    p_user_id bigint,
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
        p_user_id => NULL,
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
        user_id,
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
        NULLIF(p_user_id, 0),
        NULLIF(p_customer_id, 0),
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



-- =========================================================
-- Updated seat_hold_getbyid: JOINs both sys_user and customer
-- =========================================================

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
        h.user_id,
        u.username,
        u.full_name,
        h.customer_id,
        c.username    AS customer_username,
        c.full_name   AS customer_full_name,
        c.email       AS customer_email,
        c.phone       AS customer_phone,
        h.status,
        h.hold_started_at,
        h.hold_expires_at,
        h.released_at,
        h.release_reason,
        h.created_at
    FROM ticketing.seat_hold h
    LEFT JOIN ticketing."event" e
        ON e.event_id = h.event_id
    LEFT JOIN ticketing.sys_user u
        ON u.user_id = h.user_id
    LEFT JOIN ticketing.customer c
        ON c.customer_id = h.customer_id
    WHERE h.hold_id = p_hold_id;

    RETURN v_out;
END;
$function$;



-- =========================================================
-- Updated seat_hold_getpagedlist: JOINs both sys_user and customer
-- =========================================================

CREATE OR REPLACE FUNCTION ticketing.seat_hold_getpagedlist(
    p_pagesize integer,
    p_offset integer,
    p_keysearch varchar,
    p_event_id bigint,
    p_user_id bigint,
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
        p_user_id => -1,
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
            h.user_id,
            u.username,
            u.full_name,
            h.customer_id,
            c.username    AS customer_username,
            c.full_name   AS customer_full_name,
            c.email       AS customer_email,
            c.phone       AS customer_phone,
            h.status,
            h.hold_started_at,
            h.hold_expires_at,
            h.released_at,
            h.release_reason,
            h.created_at
        FROM ticketing.seat_hold h
        LEFT JOIN ticketing."event" e
            ON e.event_id = h.event_id
        LEFT JOIN ticketing.sys_user u
            ON u.user_id = h.user_id
        LEFT JOIN ticketing.customer c
            ON c.customer_id = h.customer_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(h.hold_code) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(u.username, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(u.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.username, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_event_id = -1
                OR h.event_id = p_event_id
              )
          AND (
                p_user_id = -1
                OR h.user_id = p_user_id
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
