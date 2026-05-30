-- =========================================================
-- MODULE: EVENT_PUBLISH_LOG
-- TABLE : ticketing.event_publish_log
-- NOTE  : refcursor, only write/read data
-- =========================================================

-- DROP FUNCTION IF EXISTS ticketing.event_publish_log_insert(bigint, varchar, varchar, varchar, bigint, varchar);
-- DROP FUNCTION IF EXISTS ticketing.event_publish_log_getbyid(bigint);
-- DROP FUNCTION IF EXISTS ticketing.event_publish_log_getpagedlist(integer, integer, varchar, bigint, varchar, varchar);



CREATE OR REPLACE FUNCTION ticketing.event_publish_log_insert(
    p_event_id bigint,
    p_action varchar,
    p_old_status varchar,
    p_new_status varchar,
    p_changed_by bigint,
    p_note varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.event_publish_log_insert(
        p_event_id => 1,
        p_action => 'publish',
        p_old_status => 'draft',
        p_new_status => 'published',
        p_changed_by => 1,
        p_note => NULL
    );
    FETCH ALL IN "event_publish_log_insert";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_publish_log_insert';
    v_event_publish_log_id bigint;
BEGIN
    INSERT INTO ticketing.event_publish_log
    (
        event_id,
        action,
        old_status,
        new_status,
        changed_by,
        changed_at,
        note
    )
    VALUES
    (
        p_event_id,
        p_action,
        nullif(trim(coalesce(p_old_status, '')), ''),
        p_new_status,
        p_changed_by,
        now(),
        nullif(trim(coalesce(p_note, '')), '')
    )
    RETURNING event_publish_log_id INTO v_event_publish_log_id;

    OPEN v_out FOR
    SELECT v_event_publish_log_id AS event_publish_log_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.event_publish_log_getbyid(
    p_event_publish_log_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.event_publish_log_getbyid(1);
    FETCH ALL IN <cursor_name>;
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_publish_log_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        epl.event_publish_log_id,
        epl.event_id,
        e.event_code,
        e.event_name,
        epl.action,
        epl.old_status,
        epl.new_status,
        epl.changed_by,
        su.full_name AS changed_by_name,
        epl.changed_at,
        epl.note
    FROM ticketing.event_publish_log epl
    LEFT JOIN ticketing."event" e
        ON e.event_id = epl.event_id
    LEFT JOIN ticketing.sys_user su
        ON su.user_id = epl.changed_by
    WHERE epl.event_publish_log_id = p_event_publish_log_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.event_publish_log_getpagedlist(
    p_pagesize integer,
    p_offset integer,
    p_keysearch varchar,
    p_event_id bigint,
    p_action varchar,
    p_new_status varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.event_publish_log_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_event_id => -1,
        p_action => '',
        p_new_status => ''
    );
    FETCH ALL IN <cursor_name>;
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_publish_log_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY epl.changed_at DESC, epl.event_publish_log_id DESC
            ) AS row_index,
            COUNT(*) OVER () AS row_total,

            epl.event_publish_log_id,
            epl.event_id,
            e.event_code,
            e.event_name,
            epl.action,
            epl.old_status,
            epl.new_status,
            epl.changed_by,
            su.full_name AS changed_by_name,
            epl.changed_at,
            epl.note
        FROM ticketing.event_publish_log epl
        LEFT JOIN ticketing."event" e
            ON e.event_id = epl.event_id
        LEFT JOIN ticketing.sys_user su
            ON su.user_id = epl.changed_by
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(su.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_event_id = -1
                OR epl.event_id = p_event_id
              )
          AND (
                trim(coalesce(p_action, '')) = ''
                OR epl.action = p_action
              )
          AND (
                trim(coalesce(p_new_status, '')) = ''
                OR epl.new_status = p_new_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$function$;
