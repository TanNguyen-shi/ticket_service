-- =========================================================
-- MODULE: CUSTOMER
-- TABLE : ticketing.customer
-- NOTE  : refcursor, only write/read data
-- USAGE : External (web client) user accounts, separate from sys_user
-- =========================================================

-- DDL: Run once to create table and indexes
-- CREATE TABLE ticketing.customer (
--     customer_id   BIGSERIAL PRIMARY KEY,
--     customer_code VARCHAR(50) UNIQUE NOT NULL,
--     username      VARCHAR(50) UNIQUE NOT NULL,
--     email         VARCHAR(100),
--     phone         VARCHAR(20),
--     password_hash VARCHAR(255) NOT NULL,
--     full_name     VARCHAR(100) NOT NULL,
--     avatar_url    VARCHAR(255),
--     status        VARCHAR(20) NOT NULL DEFAULT 'active',
--     email_verified BOOLEAN DEFAULT false,
--     last_login_at TIMESTAMP WITHOUT TIME ZONE,
--     created_at    TIMESTAMP WITHOUT TIME ZONE DEFAULT now(),
--     updated_at    TIMESTAMP WITHOUT TIME ZONE,
--     is_deleted    BOOLEAN DEFAULT false
-- );
-- CREATE UNIQUE INDEX idx_customer_username ON ticketing.customer(username) WHERE is_deleted = false;
-- CREATE UNIQUE INDEX idx_customer_email ON ticketing.customer(email) WHERE is_deleted = false AND email IS NOT NULL;
-- CREATE INDEX idx_customer_status ON ticketing.customer(status);

-- DROP FUNCTION IF EXISTS ticketing.customer_check(bigint, varchar, varchar);
-- DROP FUNCTION IF EXISTS ticketing.customer_insert(varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar);
-- DROP FUNCTION IF EXISTS ticketing.customer_getbyusername(varchar);
-- DROP FUNCTION IF EXISTS ticketing.customer_getbyid(bigint);
-- DROP FUNCTION IF EXISTS ticketing.customer_updatelastlogin(bigint);



CREATE OR REPLACE FUNCTION ticketing.customer_check(
    p_customer_id bigint DEFAULT 0,
    p_username varchar DEFAULT '',
    p_email varchar DEFAULT ''
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
DECLARE
    v_out refcursor := 'customer_check';
    v_exists integer := 0;
BEGIN
    SELECT CASE
        WHEN EXISTS
        (
            SELECT 1
            FROM ticketing.customer c
            WHERE c.is_deleted = false
              AND (
                    (trim(p_username) <> '' AND c.username = trim(p_username))
                    OR
                    (trim(p_email) <> '' AND c.email = trim(p_email))
                  )
              AND (COALESCE(p_customer_id, 0) = 0 OR c.customer_id <> p_customer_id)
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



CREATE OR REPLACE FUNCTION ticketing.customer_insert(
    p_customer_code varchar,
    p_username varchar,
    p_email varchar,
    p_phone varchar,
    p_password_hash varchar,
    p_full_name varchar,
    p_avatar_url varchar,
    p_status varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.customer_insert(
        p_customer_code => 'CUS-ABC12345678901',
        p_username => 'john_doe',
        p_email => 'john@example.com',
        p_phone => '0901234567',
        p_password_hash => '$2a$11$...',
        p_full_name => 'John Doe',
        p_avatar_url => NULL,
        p_status => 'active'
    );
    FETCH ALL IN "customer_insert";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'customer_insert';
    v_customer_id bigint;
BEGIN
    INSERT INTO ticketing.customer
    (
        customer_code,
        username,
        email,
        phone,
        password_hash,
        full_name,
        avatar_url,
        status,
        email_verified,
        created_at,
        is_deleted
    )
    VALUES
    (
        trim(p_customer_code),
        trim(p_username),
        NULLIF(trim(p_email), ''),
        NULLIF(trim(p_phone), ''),
        p_password_hash,
        trim(p_full_name),
        NULLIF(trim(p_avatar_url), ''),
        COALESCE(NULLIF(trim(p_status), ''), 'active'),
        false,
        now(),
        false
    )
    RETURNING customer_id INTO v_customer_id;

    OPEN v_out FOR
    SELECT v_customer_id AS customer_id;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.customer_getbyusername(
    p_username varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.customer_getbyusername('john_doe');
    FETCH ALL IN "customer_getbyusername";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'customer_getbyusername';
BEGIN
    OPEN v_out FOR
    SELECT
        c.customer_id,
        c.customer_code,
        c.username,
        c.email,
        c.phone,
        c.password_hash,
        c.full_name,
        c.avatar_url,
        c.status,
        c.email_verified,
        c.last_login_at,
        c.created_at,
        c.updated_at,
        c.is_deleted
    FROM ticketing.customer c
    WHERE c.username = trim(p_username)
    LIMIT 1;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.customer_getbyid(
    p_customer_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.customer_getbyid(1);
    FETCH ALL IN "customer_getbyid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'customer_getbyid';
BEGIN
    OPEN v_out FOR
    SELECT
        c.customer_id,
        c.customer_code,
        c.username,
        c.email,
        c.phone,
        c.password_hash,
        c.full_name,
        c.avatar_url,
        c.status,
        c.email_verified,
        c.last_login_at,
        c.created_at,
        c.updated_at,
        c.is_deleted
    FROM ticketing.customer c
    WHERE c.customer_id = p_customer_id
      AND c.is_deleted = false;

    RETURN v_out;
END;
$function$;



CREATE OR REPLACE FUNCTION ticketing.customer_updatelastlogin(
    p_customer_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.customer_updatelastlogin(1);
    FETCH ALL IN "customer_updatelastlogin";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'customer_updatelastlogin';
BEGIN
    UPDATE ticketing.customer
    SET
        last_login_at = now(),
        updated_at = now()
    WHERE customer_id = p_customer_id;

    OPEN v_out FOR
    SELECT p_customer_id AS customer_id;

    RETURN v_out;
END;
$function$;
