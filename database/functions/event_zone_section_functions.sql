-- =========================================================
-- MODULE: EVENT_ZONE_SECTION
-- TABLE : ticketing.event_zone_section
-- NOTE  : store function chi ghi/lay du lieu, khong xu ly business logic
-- =========================================================

-- DROP FUNCTION IF EXISTS ticketing.event_zone_section_insert(int8, int8, int8, int8);
-- DROP FUNCTION IF EXISTS ticketing.event_zone_section_update(int8, int8, int8, int8, int8);
-- DROP FUNCTION IF EXISTS ticketing.event_zone_section_delete(int8, int8);
-- DROP FUNCTION IF EXISTS ticketing.event_zone_section_getbyid(int8);
-- DROP FUNCTION IF EXISTS ticketing.event_zone_section_getbyeventzoneid(int8);
-- DROP FUNCTION IF EXISTS ticketing.event_zone_section_getbyeventid(int8);
-- DROP FUNCTION IF EXISTS ticketing.event_zone_section_getpagedlist(int4, int4, varchar, int8, int8, int8);

CREATE OR REPLACE FUNCTION ticketing.event_zone_section_insert(
    p_event_id bigint,
    p_event_zone_id bigint,
    p_section_id bigint,
    p_created_by bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_insert(
        p_event_id => 1,
        p_event_zone_id => 1,
        p_section_id => 1,
        p_created_by => 1
    );
    FETCH ALL IN "event_zone_section_insert";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_insert';
    v_event_zone_section_id bigint;
BEGIN
    INSERT INTO ticketing.event_zone_section
    (
        event_id,
        event_zone_id,
        section_id,
        created_by,
        created_at,
        is_deleted
    )
    VALUES
    (
        p_event_id,
        p_event_zone_id,
        p_section_id,
        p_created_by,
        now(),
        false
    )
    RETURNING event_zone_section_id INTO v_event_zone_section_id;

    OPEN v_out FOR
    SELECT v_event_zone_section_id AS event_zone_section_id;

    RETURN v_out;
END;
$function$;

CREATE OR REPLACE FUNCTION ticketing.event_zone_section_update(
    p_event_zone_section_id bigint,
    p_event_id bigint,
    p_event_zone_id bigint,
    p_section_id bigint,
    p_updated_by bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_update(
        p_event_zone_section_id => 1,
        p_event_id => 1,
        p_event_zone_id => 1,
        p_section_id => 2,
        p_updated_by => 1
    );
    FETCH ALL IN "event_zone_section_update";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_update';
BEGIN
    UPDATE ticketing.event_zone_section
    SET
        event_id = p_event_id,
        event_zone_id = p_event_zone_id,
        section_id = p_section_id,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE event_zone_section_id = p_event_zone_section_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_event_zone_section_id AS event_zone_section_id;

    RETURN v_out;
END;
$function$;

CREATE OR REPLACE FUNCTION ticketing.event_zone_section_delete(
    p_event_zone_section_id bigint,
    p_updated_by bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_delete(1, 1);
    FETCH ALL IN "event_zone_section_delete";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_delete';
BEGIN
    UPDATE ticketing.event_zone_section
    SET
        is_deleted = true,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE event_zone_section_id = p_event_zone_section_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_event_zone_section_id AS event_zone_section_id;

    RETURN v_out;
END;
$function$;

CREATE OR REPLACE FUNCTION ticketing.event_zone_section_getbyid(
    p_event_zone_section_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_getbyid(1);
    FETCH ALL IN "event_zone_section_getbyid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_getbyid';
BEGIN
    OPEN v_out FOR
    SELECT
        ezs.event_zone_section_id,
        ezs.event_id,
        e.event_code,
        e.event_name,
        ezs.event_zone_id,
        ez.zone_code,
        ez.zone_name,
        ezs.section_id,
        vs.section_code,
        vs.section_name,
        ezs.created_by,
        uc.full_name AS created_by_name,
        ezs.created_at,
        ezs.updated_by,
        uu.full_name AS updated_by_name,
        ezs.updated_at,
        ezs.is_deleted
    FROM ticketing.event_zone_section ezs
    LEFT JOIN ticketing."event" e
        ON e.event_id = ezs.event_id
    LEFT JOIN ticketing.event_zone ez
        ON ez.event_zone_id = ezs.event_zone_id
    LEFT JOIN ticketing.venue_section vs
        ON vs.section_id = ezs.section_id
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = ezs.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = ezs.updated_by
    WHERE ezs.event_zone_section_id = p_event_zone_section_id
      AND ezs.is_deleted = false;

    RETURN v_out;
END;
$function$;

CREATE OR REPLACE FUNCTION ticketing.event_zone_section_getbyeventzoneid(
    p_event_zone_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_getbyeventzoneid(1);
    FETCH ALL IN "event_zone_section_getbyeventzoneid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_getbyeventzoneid';
BEGIN
    OPEN v_out FOR
    SELECT
        ezs.event_zone_section_id,
        ezs.event_id,
        e.event_code,
        e.event_name,
        ezs.event_zone_id,
        ez.zone_code,
        ez.zone_name,
        ezs.section_id,
        vs.section_code,
        vs.section_name,
        ezs.created_by,
        uc.full_name AS created_by_name,
        ezs.created_at,
        ezs.updated_by,
        uu.full_name AS updated_by_name,
        ezs.updated_at
    FROM ticketing.event_zone_section ezs
    LEFT JOIN ticketing."event" e
        ON e.event_id = ezs.event_id
    LEFT JOIN ticketing.event_zone ez
        ON ez.event_zone_id = ezs.event_zone_id
    LEFT JOIN ticketing.venue_section vs
        ON vs.section_id = ezs.section_id
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = ezs.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = ezs.updated_by
    WHERE ezs.event_zone_id = p_event_zone_id
      AND ezs.is_deleted = false;

    RETURN v_out;
END;
$function$;

CREATE OR REPLACE FUNCTION ticketing.event_zone_section_getbyeventid(
    p_event_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_getbyeventid(1);
    FETCH ALL IN "event_zone_section_getbyeventid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_getbyeventid';
BEGIN
    OPEN v_out FOR
    SELECT
        ezs.event_zone_section_id,
        ezs.event_id,
        e.event_code,
        e.event_name,
        ezs.event_zone_id,
        ez.zone_code,
        ez.zone_name,
        ezs.section_id,
        vs.section_code,
        vs.section_name,
        ezs.created_by,
        uc.full_name AS created_by_name,
        ezs.created_at,
        ezs.updated_by,
        uu.full_name AS updated_by_name,
        ezs.updated_at
    FROM ticketing.event_zone_section ezs
    LEFT JOIN ticketing."event" e
        ON e.event_id = ezs.event_id
    LEFT JOIN ticketing.event_zone ez
        ON ez.event_zone_id = ezs.event_zone_id
    LEFT JOIN ticketing.venue_section vs
        ON vs.section_id = ezs.section_id
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = ezs.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = ezs.updated_by
    WHERE ezs.event_id = p_event_id
      AND ezs.is_deleted = false;

    RETURN v_out;
END;
$function$;

CREATE OR REPLACE FUNCTION ticketing.event_zone_section_getpagedlist(
    p_pagesize integer,
    p_offset integer,
    p_keysearch varchar,
    p_event_id bigint,
    p_event_zone_id bigint,
    p_section_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_event_id => -1,
        p_event_zone_id => -1,
        p_section_id => -1
    );
    FETCH ALL IN "event_zone_section_getpagedlist";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_getpagedlist';
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY ezs.created_at DESC, ezs.event_zone_section_id DESC) AS row_index,
            COUNT(*) OVER () AS row_total,
            ezs.event_zone_section_id,
            ezs.event_id,
            e.event_code,
            e.event_name,
            ezs.event_zone_id,
            ez.zone_code,
            ez.zone_name,
            ezs.section_id,
            vs.section_code,
            vs.section_name,
            ezs.created_by,
            uc.full_name AS created_by_name,
            ezs.created_at,
            ezs.updated_by,
            uu.full_name AS updated_by_name,
            ezs.updated_at
        FROM ticketing.event_zone_section ezs
        LEFT JOIN ticketing."event" e
            ON e.event_id = ezs.event_id
        LEFT JOIN ticketing.event_zone ez
            ON ez.event_zone_id = ezs.event_zone_id
        LEFT JOIN ticketing.venue_section vs
            ON vs.section_id = ezs.section_id
        LEFT JOIN ticketing.sys_user uc
            ON uc.user_id = ezs.created_by
        LEFT JOIN ticketing.sys_user uu
            ON uu.user_id = ezs.updated_by
        WHERE ezs.is_deleted = false
          AND (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ez.zone_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ez.zone_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(vs.section_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(vs.section_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_event_id = -1
                OR ezs.event_id = p_event_id
              )
          AND (
                p_event_zone_id = -1
                OR ezs.event_zone_id = p_event_zone_id
              )
          AND (
                p_section_id = -1
                OR ezs.section_id = p_section_id
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$function$;

