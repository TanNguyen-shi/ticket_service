-- =========================================================
-- MODULE: IDEMPOTENCY_REQUEST
-- TABLE : ticketing.idempotency_request
-- NOTE  : refcursor, only write/read data
-- USAGE : Chống duplicate requests (Idempotency Pattern)
-- =========================================================

-- DROP FUNCTION IF EXISTS ticketing.idempotency_request_check(bigint, varchar, varchar);
-- DROP FUNCTION IF EXISTS ticketing.idempotency_request_insert(varchar, varchar, bigint, varchar, varchar, text, timestamp without time zone);
-- DROP FUNCTION IF EXISTS ticketing.idempotency_request_update(bigint, varchar, varchar, bigint, varchar, varchar, text, timestamp without time zone);
-- DROP FUNCTION IF EXISTS ticketing.idempotency_request_delete(bigint);
-- DROP FUNCTION IF EXISTS ticketing.idempotency_request_getbyid(bigint);
-- DROP FUNCTION IF EXISTS ticketing.idempotency_request_getpagedlist(integer, integer, varchar, bigint, varchar, varchar);



CREATE OR REPLACE FUNCTION ticketing.idempotency_request_check(
    p_idempotency_id bigint DEFAULT 0,
    p_idempotency_key varchar DEFAULT '',
    p_request_type varchar DEFAULT ''
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
DECLARE
    v_out refcursor := 'idempotency_request_check';
    v_exists integer := 0;
BEGIN
    SELECT CASE
        WHEN EXISTS
        (
            SELECT 1
            FROM ticketing.idempotency_request ir
            WHERE ir.idempotency_key = trim(p_idempotency_key)
              AND ir.request_type = trim(p_request_type)
              AND (COALESCE(p_idempotency_id, 0) = 0 OR ir.idempotency_id <> p_idempotency_id)
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



CREATE OR REPLACE FUNCTION ticketing.idempotency_request_insert(
    p_idempotency_key varchar,
    p_request_type varchar,
    p_user_id bigint,
    p_request_hash varchar,
    p_status varchar,
    p_response_snapshot text,
    p_expired_at timestamp without time zone
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.idempotency_request_insert(
        p_idempotency_key => 'hold-evt5-user1-001',
        p_request_type => 'hold_seats',
        p_user_id => 1,
        p_request_hash => 'abc123hash',
        p_status => 'processing',
        p_response_snapshot => NULL,
        p_expired_at => (now() + interval '10 minute')::timestamp
    );
    FETCH ALL IN "idempotency_request_insert";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'idempotency_request_insert';
    v_idempotency_id bigint;
BEGIN
    INSERT INTO ticketing.idempotency_request
    (
        idempotency_key,
        request_type,
        user_id,
        request_hash,
        status,
        response_snapshot,
        created_at,
        expired_at
    )
    VALUES
    (
        trim(p_idempotency_key),
        trim(p_request_type),
        p_user_id,
        p_request_hash,
        p_status,
        p_response_snapshot,
        now(),
        p_expired_at
    )
    RETURNING idempotency_id INTO v_idempotency_id;

    OPEN v_out FOR
    SELECT v_idempotency_id AS idempotency_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.idempotency_request_update(
    p_idempotency_id bigint,
    p_idempotency_key varchar,
    p_request_type varchar,
    p_user_id bigint,
    p_request_hash varchar,
    p_status varchar,
    p_response_snapshot text,
    p_expired_at timestamp without time zone
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.idempotency_request_update(
        p_idempotency_id => 1,
        p_idempotency_key => 'hold-evt5-user1-001',
        p_request_type => 'hold_seats',
        p_user_id => 1,
        p_request_hash => 'abc123hash',
        p_status => 'completed',
        p_response_snapshot => '{"hold_id":1001}',
        p_expired_at => (now() + interval '10 minute')::timestamp
    );
    FETCH ALL IN "idempotency_request_update";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'idempotency_request_update';
BEGIN
    UPDATE ticketing.idempotency_request
    SET
        idempotency_key = trim(p_idempotency_key),
        request_type = trim(p_request_type),
        user_id = p_user_id,
        request_hash = p_request_hash,
        status = p_status,
        response_snapshot = p_response_snapshot,
        expired_at = p_expired_at
    WHERE idempotency_id = p_idempotency_id;

    OPEN v_out FOR
    SELECT p_idempotency_id AS idempotency_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.idempotency_request_delete(
    p_idempotency_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.idempotency_request_delete(1);
    FETCH ALL IN "idempotency_request_delete";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'idempotency_request_delete';
BEGIN
    DELETE FROM ticketing.idempotency_request
    WHERE idempotency_id = p_idempotency_id;

    OPEN v_out FOR
    SELECT p_idempotency_id AS idempotency_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.idempotency_request_getbyid(
    p_idempotency_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.idempotency_request_getbyid(1);
    FETCH ALL IN "idempotency_request_getbyid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'idempotency_request_getbyid';
BEGIN
    OPEN v_out FOR
    SELECT
        ir.idempotency_id,
        ir.idempotency_key,
        ir.request_type,
        ir.user_id,
        u.username,
        u.full_name,
        ir.request_hash,
        ir.status,
        ir.response_snapshot,
        ir.created_at,
        ir.expired_at
    FROM ticketing.idempotency_request ir
    LEFT JOIN ticketing.sys_user u
        ON u.user_id = ir.user_id
    WHERE ir.idempotency_id = p_idempotency_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.idempotency_request_getpagedlist(
    p_pagesize integer,
    p_offset integer,
    p_keysearch varchar,
    p_user_id bigint,
    p_request_type varchar,
    p_status varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.idempotency_request_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_user_id => -1,
        p_request_type => '',
        p_status => ''
    );
    FETCH ALL IN "idempotency_request_getpagedlist";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'idempotency_request_getpagedlist';
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY ir.created_at DESC, ir.idempotency_id DESC
            ) AS row_index,
            COUNT(*) OVER () AS row_total,

            ir.idempotency_id,
            ir.idempotency_key,
            ir.request_type,
            ir.user_id,
            u.username,
            u.full_name,
            ir.request_hash,
            ir.status,
            ir.response_snapshot,
            ir.created_at,
            ir.expired_at
        FROM ticketing.idempotency_request ir
        LEFT JOIN ticketing.sys_user u
            ON u.user_id = ir.user_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(ir.idempotency_key) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ir.request_hash, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(u.username, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(u.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_user_id = -1
                OR ir.user_id = p_user_id
              )
          AND (
                trim(coalesce(p_request_type, '')) = ''
                OR ir.request_type = p_request_type
              )
          AND (
                trim(coalesce(p_status, '')) = ''
                OR ir.status = p_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$function$;



-- =========================================================
-- ADDITIONAL FUNCTIONS FOR IDEMPOTENCY
-- =========================================================

CREATE OR REPLACE FUNCTION ticketing.idempotency_request_getbykeyandtype(
    p_idempotency_key varchar,
    p_request_type varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.idempotency_request_getbykeyandtype('hold-evt5-user1-001', 'hold_seats');
    FETCH ALL IN "idempotency_request_getbykeyandtype";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'idempotency_request_getbykeyandtype';
BEGIN
    OPEN v_out FOR
    SELECT
        ir.idempotency_id,
        ir.idempotency_key,
        ir.request_type,
        ir.user_id,
        u.username,
        u.full_name,
        ir.request_hash,
        ir.status,
        ir.response_snapshot,
        ir.created_at,
        ir.expired_at
    FROM ticketing.idempotency_request ir
    LEFT JOIN ticketing.sys_user u
        ON u.user_id = ir.user_id
    WHERE ir.idempotency_key = trim(p_idempotency_key)
      AND ir.request_type = trim(p_request_type)
    ORDER BY ir.created_at DESC
    LIMIT 1;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.idempotency_request_getbyuserid(
    p_user_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.idempotency_request_getbyuserid(1);
    FETCH ALL IN "idempotency_request_getbyuserid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'idempotency_request_getbyuserid';
BEGIN
    OPEN v_out FOR
    SELECT
        ir.idempotency_id,
        ir.idempotency_key,
        ir.request_type,
        ir.user_id,
        u.username,
        u.full_name,
        ir.request_hash,
        ir.status,
        ir.response_snapshot,
        ir.created_at,
        ir.expired_at
    FROM ticketing.idempotency_request ir
    LEFT JOIN ticketing.sys_user u
        ON u.user_id = ir.user_id
    WHERE ir.user_id = p_user_id
    ORDER BY ir.created_at DESC;

    RETURN v_out;
END;
$function$;

