-- =========================================================
-- MODULE: PAYMENT_TRANSACTION
-- TABLE : ticketing.payment_transaction
-- NOTE  : refcursor, only write/read data
-- =========================================================

-- DROP FUNCTION IF EXISTS ticketing.payment_transaction_check(bigint, varchar, varchar);
-- DROP FUNCTION IF EXISTS ticketing.payment_transaction_insert(bigint, varchar, varchar, varchar, numeric, varchar, timestamp without time zone, timestamp without time zone, text, text);
-- DROP FUNCTION IF EXISTS ticketing.payment_transaction_update(bigint, bigint, varchar, varchar, varchar, numeric, varchar, timestamp without time zone, timestamp without time zone, text, text);
-- DROP FUNCTION IF EXISTS ticketing.payment_transaction_delete(bigint);
-- DROP FUNCTION IF EXISTS ticketing.payment_transaction_getbyid(bigint);
-- DROP FUNCTION IF EXISTS ticketing.payment_transaction_getpagedlist(integer, integer, varchar, bigint, varchar, varchar);



CREATE OR REPLACE FUNCTION ticketing.payment_transaction_check(
    p_payment_id bigint DEFAULT 0,
    p_payment_ref varchar DEFAULT '',
    p_provider_transaction_ref varchar DEFAULT ''
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
DECLARE
    v_out refcursor := 'payment_transaction_check';
    v_payment_ref_exists integer := 0;
    v_provider_ref_exists integer := 0;
BEGIN
    SELECT CASE
        WHEN EXISTS
        (
            SELECT 1
            FROM ticketing.payment_transaction p
            WHERE p.payment_ref = trim(p_payment_ref)
              AND (COALESCE(p_payment_id, 0) = 0 OR p.payment_id <> p_payment_id)
        )
        THEN 1 ELSE 0
    END
    INTO v_payment_ref_exists;

    SELECT CASE
        WHEN trim(coalesce(p_provider_transaction_ref, '')) <> ''
             AND EXISTS
             (
                SELECT 1
                FROM ticketing.payment_transaction p
                WHERE p.provider_transaction_ref = trim(p_provider_transaction_ref)
                  AND (COALESCE(p_payment_id, 0) = 0 OR p.payment_id <> p_payment_id)
             )
        THEN 1 ELSE 0
    END
    INTO v_provider_ref_exists;

    OPEN v_out FOR
    SELECT
        v_payment_ref_exists AS payment_ref_exists,
        v_provider_ref_exists AS provider_transaction_ref_exists;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.payment_transaction_insert(
    p_order_id bigint,
    p_payment_provider varchar,
    p_payment_ref varchar,
    p_provider_transaction_ref varchar,
    p_amount numeric,
    p_payment_status varchar,
    p_requested_at timestamp without time zone,
    p_confirmed_at timestamp without time zone,
    p_raw_request_payload text,
    p_raw_callback_payload text
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_transaction_insert(
        p_order_id => 1,
        p_payment_provider => 'mock',
        p_payment_ref => 'PAY20260001',
        p_provider_transaction_ref => 'MOCK_TXN_0001',
        p_amount => 1500000,
        p_payment_status => 'pending',
        p_requested_at => now()::timestamp,
        p_confirmed_at => NULL,
        p_raw_request_payload => '{"request":"demo"}',
        p_raw_callback_payload => NULL
    );
    FETCH ALL IN "payment_transaction_insert";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_transaction_insert';
    v_payment_id bigint;
BEGIN
    INSERT INTO ticketing.payment_transaction
    (
        order_id,
        payment_provider,
        payment_ref,
        provider_transaction_ref,
        amount,
        payment_status,
        requested_at,
        confirmed_at,
        raw_request_payload,
        raw_callback_payload,
        created_at,
        updated_at
    )
    VALUES
    (
        p_order_id,
        p_payment_provider,
        trim(p_payment_ref),
        nullif(trim(coalesce(p_provider_transaction_ref, '')), ''),
        p_amount,
        p_payment_status,
        p_requested_at,
        p_confirmed_at,
        p_raw_request_payload,
        p_raw_callback_payload,
        now(),
        NULL
    )
    RETURNING payment_id INTO v_payment_id;

    OPEN v_out FOR
    SELECT v_payment_id AS payment_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.payment_transaction_update(
    p_payment_id bigint,
    p_order_id bigint,
    p_payment_provider varchar,
    p_payment_ref varchar,
    p_provider_transaction_ref varchar,
    p_amount numeric,
    p_payment_status varchar,
    p_requested_at timestamp without time zone,
    p_confirmed_at timestamp without time zone,
    p_raw_request_payload text,
    p_raw_callback_payload text
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_transaction_update(
        p_payment_id => 1,
        p_order_id => 1,
        p_payment_provider => 'mock',
        p_payment_ref => 'PAY20260001',
        p_provider_transaction_ref => 'MOCK_TXN_0001',
        p_amount => 1500000,
        p_payment_status => 'success',
        p_requested_at => '2026-01-01 10:00:00'::timestamp,
        p_confirmed_at => '2026-01-01 10:02:00'::timestamp,
        p_raw_request_payload => '{"request":"demo"}',
        p_raw_callback_payload => '{"callback":"ok"}'
    );
    FETCH ALL IN "payment_transaction_update";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_transaction_update';
BEGIN
    UPDATE ticketing.payment_transaction
    SET
        order_id = p_order_id,
        payment_provider = p_payment_provider,
        payment_ref = trim(p_payment_ref),
        provider_transaction_ref = nullif(trim(coalesce(p_provider_transaction_ref, '')), ''),
        amount = p_amount,
        payment_status = p_payment_status,
        requested_at = p_requested_at,
        confirmed_at = p_confirmed_at,
        raw_request_payload = p_raw_request_payload,
        raw_callback_payload = p_raw_callback_payload,
        updated_at = now()
    WHERE payment_id = p_payment_id;

    OPEN v_out FOR
    SELECT p_payment_id AS payment_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.payment_transaction_delete(
    p_payment_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_transaction_delete(1);
    FETCH ALL IN "payment_transaction_delete";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_transaction_delete';
BEGIN
    DELETE FROM ticketing.payment_transaction
    WHERE payment_id = p_payment_id;

    OPEN v_out FOR
    SELECT p_payment_id AS payment_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.payment_transaction_getbyid(
    p_payment_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_transaction_getbyid(1);
    FETCH ALL IN "payment_transaction_getbyid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_transaction_getbyid';
BEGIN
    OPEN v_out FOR
    SELECT
        p.payment_id,
        p.order_id,
        o.order_code,
        o.event_id,
        e.event_code,
        e.event_name,
        o.user_id,
        u.username,
        u.full_name,

        p.payment_provider,
        p.payment_ref,
        p.provider_transaction_ref,
        p.amount,
        p.payment_status,
        p.requested_at,
        p.confirmed_at,
        p.raw_request_payload,
        p.raw_callback_payload,
        p.created_at,
        p.updated_at
    FROM ticketing.payment_transaction p
    LEFT JOIN ticketing.ticket_order o
        ON o.order_id = p.order_id
    LEFT JOIN ticketing."event" e
        ON e.event_id = o.event_id
    LEFT JOIN ticketing.sys_user u
        ON u.user_id = o.user_id
    WHERE p.payment_id = p_payment_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.payment_transaction_getpagedlist(
    p_pagesize integer,
    p_offset integer,
    p_keysearch varchar,
    p_order_id bigint,
    p_payment_provider varchar,
    p_payment_status varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_transaction_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_order_id => -1,
        p_payment_provider => '',
        p_payment_status => ''
    );
    FETCH ALL IN "payment_transaction_getpagedlist";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_transaction_getpagedlist';
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY p.created_at DESC, p.payment_id DESC
            ) AS row_index,
            COUNT(*) OVER () AS row_total,

            p.payment_id,
            p.order_id,
            o.order_code,
            o.event_id,
            e.event_code,
            e.event_name,
            o.user_id,
            u.username,
            u.full_name,

            p.payment_provider,
            p.payment_ref,
            p.provider_transaction_ref,
            p.amount,
            p.payment_status,
            p.requested_at,
            p.confirmed_at,
            p.created_at,
            p.updated_at
        FROM ticketing.payment_transaction p
        LEFT JOIN ticketing.ticket_order o
            ON o.order_id = p.order_id
        LEFT JOIN ticketing."event" e
            ON e.event_id = o.event_id
        LEFT JOIN ticketing.sys_user u
            ON u.user_id = o.user_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(p.payment_ref) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(p.provider_transaction_ref, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(o.order_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(u.username, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(u.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_order_id = -1
                OR p.order_id = p_order_id
              )
          AND (
                trim(coalesce(p_payment_provider, '')) = ''
                OR p.payment_provider = p_payment_provider
              )
          AND (
                trim(coalesce(p_payment_status, '')) = ''
                OR p.payment_status = p_payment_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$function$;


-- =========================================================
-- ADDITIONAL FUNCTIONS FOR PAYMENT MODULE
-- =========================================================

CREATE OR REPLACE FUNCTION ticketing.payment_transaction_getbyorderid(
    p_order_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_transaction_getbyorderid(1);
    FETCH ALL IN "payment_transaction_getbyorderid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_transaction_getbyorderid';
BEGIN
    OPEN v_out FOR
    SELECT
        p.payment_id,
        p.order_id,
        o.order_code,
        o.event_id,
        e.event_code,
        e.event_name,
        o.user_id,
        u.username,
        u.full_name,

        p.payment_provider,
        p.payment_ref,
        p.provider_transaction_ref,
        p.amount,
        p.payment_status,
        p.requested_at,
        p.confirmed_at,
        p.raw_request_payload,
        p.raw_callback_payload,
        p.created_at,
        p.updated_at
    FROM ticketing.payment_transaction p
    LEFT JOIN ticketing.ticket_order o
        ON o.order_id = p.order_id
    LEFT JOIN ticketing."event" e
        ON e.event_id = o.event_id
    LEFT JOIN ticketing.sys_user u
        ON u.user_id = o.user_id
    WHERE p.order_id = p_order_id;

    RETURN v_out;
END;
$function$;



-- =========================================================
-- MODULE: PAYMENT_CALLBACK_LOG
-- TABLE : ticketing.payment_callback_log
-- NOTE  : refcursor, only write/read data
-- =========================================================

-- DROP FUNCTION IF EXISTS ticketing.payment_callback_log_insert(bigint, varchar, varchar, varchar, text, boolean, varchar, timestamp without time zone, timestamp without time zone);
-- DROP FUNCTION IF EXISTS ticketing.payment_callback_log_update(bigint, bigint, varchar, varchar, varchar, text, boolean, varchar, timestamp without time zone, timestamp without time zone);
-- DROP FUNCTION IF EXISTS ticketing.payment_callback_log_delete(bigint);
-- DROP FUNCTION IF EXISTS ticketing.payment_callback_log_getbyid(bigint);
-- DROP FUNCTION IF EXISTS ticketing.payment_callback_log_getpagedlist(integer, integer, varchar, bigint, varchar, varchar);



CREATE OR REPLACE FUNCTION ticketing.payment_callback_log_insert(
    p_payment_id bigint,
    p_payment_provider varchar,
    p_external_transaction_ref varchar,
    p_callback_signature varchar,
    p_payload text,
    p_signature_valid boolean,
    p_processed_status varchar,
    p_received_at timestamp without time zone,
    p_processed_at timestamp without time zone
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_callback_log_insert(
        p_payment_id => 1,
        p_payment_provider => 'mock',
        p_external_transaction_ref => 'MOCK_TXN_0001',
        p_callback_signature => 'abc123',
        p_payload => '{"callback":"demo"}',
        p_signature_valid => true,
        p_processed_status => 'received',
        p_received_at => now()::timestamp,
        p_processed_at => NULL
    );
    FETCH ALL IN "payment_callback_log_insert";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_callback_log_insert';
    v_callback_log_id bigint;
BEGIN
    INSERT INTO ticketing.payment_callback_log
    (
        payment_id,
        payment_provider,
        external_transaction_ref,
        callback_signature,
        payload,
        signature_valid,
        processed_status,
        received_at,
        processed_at
    )
    VALUES
    (
        p_payment_id,
        p_payment_provider,
        nullif(trim(coalesce(p_external_transaction_ref, '')), ''),
        p_callback_signature,
        p_payload,
        p_signature_valid,
        p_processed_status,
        COALESCE(p_received_at, now()::timestamp),
        p_processed_at
    )
    RETURNING callback_log_id INTO v_callback_log_id;

    OPEN v_out FOR
    SELECT v_callback_log_id AS callback_log_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.payment_callback_log_update(
    p_callback_log_id bigint,
    p_payment_id bigint,
    p_payment_provider varchar,
    p_external_transaction_ref varchar,
    p_callback_signature varchar,
    p_payload text,
    p_signature_valid boolean,
    p_processed_status varchar,
    p_received_at timestamp without time zone,
    p_processed_at timestamp without time zone
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_callback_log_update(
        p_callback_log_id => 1,
        p_payment_id => 1,
        p_payment_provider => 'mock',
        p_external_transaction_ref => 'MOCK_TXN_0001',
        p_callback_signature => 'abc123',
        p_payload => '{"callback":"processed"}',
        p_signature_valid => true,
        p_processed_status => 'processed',
        p_received_at => '2026-01-01 10:00:00'::timestamp,
        p_processed_at => '2026-01-01 10:01:00'::timestamp
    );
    FETCH ALL IN "payment_callback_log_update";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_callback_log_update';
BEGIN
    UPDATE ticketing.payment_callback_log
    SET
        payment_id = p_payment_id,
        payment_provider = p_payment_provider,
        external_transaction_ref = nullif(trim(coalesce(p_external_transaction_ref, '')), ''),
        callback_signature = p_callback_signature,
        payload = p_payload,
        signature_valid = p_signature_valid,
        processed_status = p_processed_status,
        received_at = p_received_at,
        processed_at = p_processed_at
    WHERE callback_log_id = p_callback_log_id;

    OPEN v_out FOR
    SELECT p_callback_log_id AS callback_log_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.payment_callback_log_delete(
    p_callback_log_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_callback_log_delete(1);
    FETCH ALL IN "payment_callback_log_delete";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_callback_log_delete';
BEGIN
    DELETE FROM ticketing.payment_callback_log
    WHERE callback_log_id = p_callback_log_id;

    OPEN v_out FOR
    SELECT p_callback_log_id AS callback_log_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.payment_callback_log_getbyid(
    p_callback_log_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_callback_log_getbyid(1);
    FETCH ALL IN "payment_callback_log_getbyid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_callback_log_getbyid';
BEGIN
    OPEN v_out FOR
    SELECT
        c.callback_log_id,
        c.payment_id,
        p.payment_ref,
        p.provider_transaction_ref,
        p.order_id,
        o.order_code,

        c.payment_provider,
        c.external_transaction_ref,
        c.callback_signature,
        c.payload,
        c.signature_valid,
        c.processed_status,
        c.received_at,
        c.processed_at
    FROM ticketing.payment_callback_log c
    LEFT JOIN ticketing.payment_transaction p
        ON p.payment_id = c.payment_id
    LEFT JOIN ticketing.ticket_order o
        ON o.order_id = p.order_id
    WHERE c.callback_log_id = p_callback_log_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.payment_callback_log_getpagedlist(
    p_pagesize integer,
    p_offset integer,
    p_keysearch varchar,
    p_payment_id bigint,
    p_payment_provider varchar,
    p_processed_status varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_callback_log_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_payment_id => -1,
        p_payment_provider => '',
        p_processed_status => ''
    );
    FETCH ALL IN "payment_callback_log_getpagedlist";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_callback_log_getpagedlist';
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY c.received_at DESC, c.callback_log_id DESC
            ) AS row_index,
            COUNT(*) OVER () AS row_total,

            c.callback_log_id,
            c.payment_id,
            p.payment_ref,
            p.provider_transaction_ref,
            p.order_id,
            o.order_code,

            c.payment_provider,
            c.external_transaction_ref,
            c.signature_valid,
            c.processed_status,
            c.received_at,
            c.processed_at
        FROM ticketing.payment_callback_log c
        LEFT JOIN ticketing.payment_transaction p
            ON p.payment_id = c.payment_id
        LEFT JOIN ticketing.ticket_order o
            ON o.order_id = p.order_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(coalesce(p.payment_ref, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(p.provider_transaction_ref, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.external_transaction_ref, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(o.order_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_payment_id = -1
                OR c.payment_id = p_payment_id
              )
          AND (
                trim(coalesce(p_payment_provider, '')) = ''
                OR c.payment_provider = p_payment_provider
              )
          AND (
                trim(coalesce(p_processed_status, '')) = ''
                OR c.processed_status = p_processed_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.payment_callback_log_getbypaymentid(
    p_payment_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.payment_callback_log_getbypaymentid(1);
    FETCH ALL IN "payment_callback_log_getbypaymentid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'payment_callback_log_getbypaymentid';
BEGIN
    OPEN v_out FOR
    SELECT
        c.callback_log_id,
        c.payment_id,
        p.payment_ref,
        p.provider_transaction_ref,
        p.order_id,
        o.order_code,

        c.payment_provider,
        c.external_transaction_ref,
        c.callback_signature,
        c.payload,
        c.signature_valid,
        c.processed_status,
        c.received_at,
        c.processed_at
    FROM ticketing.payment_callback_log c
    LEFT JOIN ticketing.payment_transaction p
        ON p.payment_id = c.payment_id
    LEFT JOIN ticketing.ticket_order o
        ON o.order_id = p.order_id
    WHERE c.payment_id = p_payment_id;

    RETURN v_out;
END;
$function$;

