--
-- PostgreSQL database dump
--

\restrict DDNLY0z1PqID8ycWoF7l5yPYjdKsaeUlrODwJww3gLP29DmSYltU5LNtpRO1HZW

-- Dumped from database version 16.13 (Debian 16.13-1.pgdg13+1)
-- Dumped by pg_dump version 16.13 (Debian 16.13-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: ticketing; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA ticketing;


--
-- Name: customer_check(bigint, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.customer_check(p_customer_id bigint DEFAULT 0, p_username character varying DEFAULT ''::character varying, p_email character varying DEFAULT ''::character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'customer_check_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: customer_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.customer_getbyid(p_customer_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.customer_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'customer_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: customer_getbyusername(character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.customer_getbyusername(p_username character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.customer_getbyusername('john_doe');
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'customer_getbyusername_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: customer_insert(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.customer_insert(p_customer_code character varying, p_username character varying, p_email character varying, p_phone character varying, p_password_hash character varying, p_full_name character varying, p_avatar_url character varying, p_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $_$
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
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'customer_insert_' || replace(gen_random_uuid()::text, '-', '_');
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
$_$;


--
-- Name: customer_updatelastlogin(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.customer_updatelastlogin(p_customer_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.customer_updatelastlogin(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'customer_updatelastlogin_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: event_delete(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_delete(p_event_id bigint, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_delete(1, 1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing."event"
    SET
        is_deleted = true,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE event_id = p_event_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_event_id AS event_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_get_featured_client(integer); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_get_featured_client(p_limit integer DEFAULT 8) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_get_featured_client(8);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_get_featured_client_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        e.event_id,
        e.event_code,
        e.event_name,
        e.description,
        e.venue_id,
        v.venue_code,
        v.venue_name,
        v.city,
        v.country,
        e.banner_url,
        e.start_time,
        e.end_time,
        e.sale_start_time,
        e.sale_end_time,
        e.status,
        e.published_at,
        e.on_sale_at,
        e.is_featured,
        e.is_trending,
        e.display_order
    FROM ticketing."event" e
    LEFT JOIN ticketing.venue v
        ON v.venue_id = e.venue_id
    WHERE e.is_deleted = false
      AND e.is_featured = true
      AND e.status IN ('published', 'on_sale')
    ORDER BY
        e.display_order ASC,
        e.start_time ASC,
        e.event_id DESC
    LIMIT p_limit;

    RETURN v_out;
END;
$$;


--
-- Name: event_get_trending_client(integer); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_get_trending_client(p_limit integer DEFAULT 12) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_get_trending_client(12);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_get_trending_client_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        e.event_id,
        e.event_code,
        e.event_name,
        e.description,
        e.venue_id,
        v.venue_code,
        v.venue_name,
        v.city,
        v.country,
        e.banner_url,
        e.start_time,
        e.end_time,
        e.sale_start_time,
        e.sale_end_time,
        e.status,
        e.published_at,
        e.on_sale_at,
        e.is_featured,
        e.is_trending,
        e.display_order
    FROM ticketing."event" e
    LEFT JOIN ticketing.venue v
        ON v.venue_id = e.venue_id
    WHERE e.is_deleted = false
      AND e.is_trending = true
      AND e.status IN ('published', 'on_sale')
    ORDER BY
        e.is_featured DESC,
        e.display_order ASC,
        e.start_time ASC,
        e.event_id DESC
    LIMIT p_limit;

    RETURN v_out;
END;
$$;


--
-- Name: event_get_upcoming_client(integer, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_get_upcoming_client(p_limit integer DEFAULT 12, p_now timestamp without time zone DEFAULT now()) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_get_upcoming_client(12, now()::timestamp);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_get_upcoming_client_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        e.event_id,
        e.event_code,
        e.event_name,
        e.description,
        e.venue_id,
        v.venue_code,
        v.venue_name,
        v.city,
        v.country,
        e.banner_url,
        e.start_time,
        e.end_time,
        e.sale_start_time,
        e.sale_end_time,
        e.status,
        e.published_at,
        e.on_sale_at,
        e.is_featured,
        e.is_trending,
        e.display_order
    FROM ticketing."event" e
    LEFT JOIN ticketing.venue v
        ON v.venue_id = e.venue_id
    WHERE e.is_deleted = false
      AND e.status IN ('published', 'on_sale')
      AND e.start_time >= p_now
    ORDER BY
        e.is_featured DESC,
        e.is_trending DESC,
        e.display_order ASC,
        e.start_time ASC,
        e.event_id DESC
    LIMIT p_limit;

    RETURN v_out;
END;
$$;


--
-- Name: event_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_getbyid(p_event_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        e.event_id,
        e.event_code,
        e.event_name,
        e.description,
        e.venue_id,
        v.venue_code,
        v.venue_name,
        v.address_line,
        v.city,
        v.country,
        e.banner_url,
        e.start_time,
        e.end_time,
        e.sale_start_time,
        e.sale_end_time,
        e.status,
        e.published_at,
        e.on_sale_at,
        e.is_featured,
        e.is_trending,
        e.display_order,
        e.created_by,
        uc.full_name AS created_by_name,
        e.created_at,
        e.updated_by,
        uu.full_name AS updated_by_name,
        e.updated_at,
        e.is_deleted
    FROM ticketing."event" e
    LEFT JOIN ticketing.venue v
        ON v.venue_id = e.venue_id
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = e.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = e.updated_by
    WHERE e.event_id = p_event_id
      AND e.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: event_getpagedlist(integer, integer, character varying, character varying, bigint, boolean, boolean); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_status character varying, p_venue_id bigint, p_is_featured boolean, p_is_trending boolean) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_status => '',
        p_venue_id => -1,
        p_is_featured => NULL,
        p_is_trending => NULL
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY
                    e.display_order ASC,
                    e.is_featured DESC,
                    e.is_trending DESC,
                    e.created_at DESC,
                    e.event_id DESC
            ) AS row_index,
            COUNT(*) OVER () AS row_total,

            e.event_id,
            e.event_code,
            e.event_name,
            e.description,
            e.venue_id,
            v.venue_code,
            v.venue_name,
            e.banner_url,
            e.start_time,
            e.end_time,
            e.sale_start_time,
            e.sale_end_time,
            e.status,
            e.published_at,
            e.on_sale_at,
            e.is_featured,
            e.is_trending,
            e.display_order,
            e.created_by,
            uc.full_name AS created_by_name,
            e.created_at,
            e.updated_by,
            uu.full_name AS updated_by_name,
            e.updated_at
        FROM ticketing."event" e
        LEFT JOIN ticketing.venue v
            ON v.venue_id = e.venue_id
        LEFT JOIN ticketing.sys_user uc
            ON uc.user_id = e.created_by
        LEFT JOIN ticketing.sys_user uu
            ON uu.user_id = e.updated_by
        WHERE e.is_deleted = false
          AND (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(e.event_code) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(e.event_name) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.description, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(v.venue_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                trim(coalesce(p_status, '')) = ''
                OR e.status = p_status
              )
          AND (
                p_venue_id = -1
                OR e.venue_id = p_venue_id
              )
          AND (
                p_is_featured IS NULL
                OR e.is_featured = p_is_featured
              )
          AND (
                p_is_trending IS NULL
                OR e.is_trending = p_is_trending
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: event_getpagedlist(integer, integer, character varying, character varying, bigint, integer, integer); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_status character varying, p_venue_id bigint, p_is_featured integer DEFAULT '-1'::integer, p_is_trending integer DEFAULT '-1'::integer) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_status => '',
        p_venue_id => -1,
        p_is_featured => -1,
        p_is_trending => -1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY
                    e.display_order ASC,
                    e.is_featured DESC,
                    e.is_trending DESC,
                    e.created_at DESC,
                    e.event_id DESC
            ) AS row_index,
            COUNT(*) OVER () AS row_total,

            e.event_id,
            e.event_code,
            e.event_name,
            e.description,
            e.venue_id,
            v.venue_code,
            v.venue_name,
            e.banner_url,
            e.start_time,
            e.end_time,
            e.sale_start_time,
            e.sale_end_time,
            e.status,
            e.published_at,
            e.on_sale_at,
            e.is_featured,
            e.is_trending,
            e.display_order,
            e.created_by,
            uc.full_name AS created_by_name,
            e.created_at,
            e.updated_by,
            uu.full_name AS updated_by_name,
            e.updated_at
        FROM ticketing."event" e
        LEFT JOIN ticketing.venue v
            ON v.venue_id = e.venue_id
        LEFT JOIN ticketing.sys_user uc
            ON uc.user_id = e.created_by
        LEFT JOIN ticketing.sys_user uu
            ON uu.user_id = e.updated_by
        WHERE e.is_deleted = false
          AND (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(e.event_code) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(e.event_name) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.description, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(v.venue_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                trim(coalesce(p_status, '')) = ''
                OR e.status = p_status
              )
          AND (
                p_venue_id = -1
                OR e.venue_id = p_venue_id
              )
          AND (
                p_is_featured = -1
                OR (p_is_featured = 1 AND e.is_featured = true)
                OR (p_is_featured = 0 AND e.is_featured = false)
              )
          AND (
                p_is_trending = -1
                OR (p_is_trending = 1 AND e.is_trending = true)
                OR (p_is_trending = 0 AND e.is_trending = false)
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: event_insert(character varying, character varying, text, bigint, character varying, timestamp without time zone, timestamp without time zone, timestamp without time zone, timestamp without time zone, character varying, timestamp without time zone, timestamp without time zone, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_insert(p_event_code character varying, p_event_name character varying, p_description text, p_venue_id bigint, p_banner_url character varying, p_start_time timestamp without time zone, p_end_time timestamp without time zone, p_sale_start_time timestamp without time zone, p_sale_end_time timestamp without time zone, p_status character varying, p_published_at timestamp without time zone, p_on_sale_at timestamp without time zone, p_created_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_insert(
        p_event_code => 'EVT_QK7_001',
        p_event_name => 'Sky Tour Live Concert 2026 - TP.HCM',
        p_description => 'Demo event',
        p_venue_id => 13,
        p_banner_url => 'test',
        p_start_time => '2026-03-28 11:03:00'::timestamp,
        p_end_time => '2026-03-29 11:03:00'::timestamp,
        p_sale_start_time => '2026-03-24 11:03:00'::timestamp,
        p_sale_end_time => '2026-03-27 11:03:00'::timestamp,
        p_status => 'draft',
        p_published_at => '2026-03-23 11:03:00'::timestamp,
        p_on_sale_at => '2026-03-24 11:03:00'::timestamp,
        p_created_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_event_id bigint;
BEGIN
    INSERT INTO ticketing."event"
    (
        event_code,
        event_name,
        description,
        venue_id,
        banner_url,
        start_time,
        end_time,
        sale_start_time,
        sale_end_time,
        status,
        published_at,
        on_sale_at,
        created_by,
        created_at,
        is_deleted
    )
    VALUES
    (
        trim(p_event_code),
        trim(p_event_name),
        p_description,
        p_venue_id,
        p_banner_url,
        p_start_time,
        p_end_time,
        p_sale_start_time,
        p_sale_end_time,
        p_status,
        p_published_at,
        p_on_sale_at,
        p_created_by,
        now(),
        false
    )
    RETURNING event_id INTO v_event_id;

    OPEN v_out FOR
    SELECT v_event_id AS event_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_insert(character varying, character varying, text, bigint, character varying, timestamp without time zone, timestamp without time zone, timestamp without time zone, timestamp without time zone, character varying, timestamp without time zone, timestamp without time zone, boolean, boolean, integer, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_insert(p_event_code character varying, p_event_name character varying, p_description text, p_venue_id bigint, p_banner_url character varying, p_start_time timestamp without time zone, p_end_time timestamp without time zone, p_sale_start_time timestamp without time zone, p_sale_end_time timestamp without time zone, p_status character varying, p_published_at timestamp without time zone, p_on_sale_at timestamp without time zone, p_is_featured boolean, p_is_trending boolean, p_display_order integer, p_created_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_insert(
        p_event_code => 'EVT001',
        p_event_name => 'Sky Tour Live Concert 2026 - TP.HCM',
        p_description => 'Demo event',
        p_venue_id => 1,
        p_banner_url => 'https://placehold.co/800x400/png',
        p_start_time => '2026-06-20 19:30:00'::timestamp,
        p_end_time => '2026-06-20 22:30:00'::timestamp,
        p_sale_start_time => '2026-05-15 10:00:00'::timestamp,
        p_sale_end_time => '2026-06-20 18:30:00'::timestamp,
        p_status => 'on_sale',
        p_published_at => '2026-05-10 09:00:00'::timestamp,
        p_on_sale_at => '2026-05-15 10:00:00'::timestamp,
        p_is_featured => true,
        p_is_trending => true,
        p_display_order => 1,
        p_created_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_event_id bigint;
BEGIN
    INSERT INTO ticketing."event"
    (
        event_code,
        event_name,
        description,
        venue_id,
        banner_url,
        start_time,
        end_time,
        sale_start_time,
        sale_end_time,
        status,
        published_at,
        on_sale_at,
        is_featured,
        is_trending,
        display_order,
        created_by,
        created_at,
        is_deleted
    )
    VALUES
    (
        trim(p_event_code),
        trim(p_event_name),
        p_description,
        p_venue_id,
        p_banner_url,
        p_start_time,
        p_end_time,
        p_sale_start_time,
        p_sale_end_time,
        p_status,
        p_published_at,
        p_on_sale_at,
        COALESCE(p_is_featured, false),
        COALESCE(p_is_trending, false),
        COALESCE(p_display_order, 0),
        p_created_by,
        now(),
        false
    )
    RETURNING event_id INTO v_event_id;

    OPEN v_out FOR
    SELECT v_event_id AS event_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_publish_log_delete(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_publish_log_delete(p_event_publish_log_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_publish_log_delete(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_publish_log_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    DELETE FROM ticketing.event_publish_log
    WHERE event_publish_log_id = p_event_publish_log_id;

    OPEN v_out FOR
    SELECT
        p_event_publish_log_id AS event_publish_log_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_publish_log_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_publish_log_getbyid(p_event_publish_log_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: event_publish_log_getpagedlist(integer, integer, character varying, bigint, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_publish_log_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_event_id bigint, p_action character varying, p_new_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: event_publish_log_insert(bigint, character varying, character varying, character varying, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_publish_log_insert(p_event_id bigint, p_action character varying, p_old_status character varying, p_new_status character varying, p_changed_by bigint, p_note character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: event_publish_log_update(bigint, bigint, character varying, character varying, character varying, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_publish_log_update(p_event_publish_log_id bigint, p_event_id bigint, p_action character varying, p_old_status character varying, p_new_status character varying, p_changed_by bigint, p_note character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_publish_log_update(
        p_event_publish_log_id => 1,
        p_event_id => 1,
        p_action => 'open_sale',
        p_old_status => 'published',
        p_new_status => 'on_sale',
        p_changed_by => 1,
        p_note => 'Update publish log'
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_publish_log_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.event_publish_log
    SET
        event_id = p_event_id,
        action = p_action,
        old_status = p_old_status,
        new_status = p_new_status,
        changed_by = p_changed_by,
        note = p_note
    WHERE event_publish_log_id = p_event_publish_log_id;

    OPEN v_out FOR
    SELECT
        p_event_publish_log_id AS event_publish_log_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_search_client(integer, integer, character varying, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_search_client(p_pagesize integer, p_offset integer, p_keysearch character varying, p_from_date timestamp without time zone, p_to_date timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_search_client(
        p_pagesize => 12,
        p_offset => 0,
        p_keysearch => '',
        p_status => '',
        p_venue_id => -1,
        p_is_featured => NULL,
        p_is_trending => NULL,
        p_from_date => NULL,
        p_to_date => NULL
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_search_client_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        e.event_id,
        e.event_code,
        e.event_name,
        e.description,
        e.venue_id,
        v.venue_code,
        v.venue_name,
        v.city,
        v.country,
        e.banner_url,
        e.start_time,
        e.end_time,
        e.sale_start_time,
        e.sale_end_time,
        e.status,
        e.published_at,
        e.on_sale_at,
        e.is_featured,
        e.is_trending,
        e.display_order
    FROM ticketing."event" e
    LEFT JOIN ticketing.venue v
        ON v.venue_id = e.venue_id
    WHERE e.is_deleted = false
      AND e.status IN ('published', 'on_sale', 'ended')
      AND (
            trim(coalesce(p_keysearch, '')) = ''
            OR e.event_code ILIKE '%' || trim(p_keysearch) || '%'
            OR e.event_name ILIKE '%' || trim(p_keysearch) || '%'
            OR coalesce(e.description, '') ILIKE '%' || trim(p_keysearch) || '%'
            OR coalesce(v.venue_name, '') ILIKE '%' || trim(p_keysearch) || '%'
            OR coalesce(v.city, '') ILIKE '%' || trim(p_keysearch) || '%'
          )
      AND (
            p_from_date IS NULL
            OR e.end_time >= p_from_date
          )
      AND (
            p_to_date IS NULL
            OR e.start_time <= p_to_date
          )
    ORDER BY
        e.is_featured DESC,
        e.is_trending DESC,
        e.display_order ASC,
        e.start_time ASC,
        e.event_id DESC
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: event_search_client(integer, integer, character varying, character varying, bigint, boolean, boolean, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_search_client(p_pagesize integer, p_offset integer, p_keysearch character varying, p_status character varying, p_venue_id bigint, p_is_featured boolean, p_is_trending boolean, p_from_date timestamp without time zone, p_to_date timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_search_client(
        p_pagesize => 12,
        p_offset => 0,
        p_keysearch => '',
        p_status => '',
        p_venue_id => -1,
        p_is_featured => NULL,
        p_is_trending => NULL,
        p_from_date => NULL,
        p_to_date => NULL
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_search_client_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        e.event_id,
        e.event_code,
        e.event_name,
        e.description,
        e.venue_id,
        v.venue_code,
        v.venue_name,
        v.city,
        v.country,
        e.banner_url,
        e.start_time,
        e.end_time,
        e.sale_start_time,
        e.sale_end_time,
        e.status,
        e.published_at,
        e.on_sale_at,
        e.is_featured,
        e.is_trending,
        e.display_order
    FROM ticketing."event" e
    LEFT JOIN ticketing.venue v
        ON v.venue_id = e.venue_id
    WHERE e.is_deleted = false
      AND e.status IN ('published', 'on_sale', 'ended')
      AND (
            trim(coalesce(p_keysearch, '')) = ''
            OR e.event_code ILIKE '%' || trim(p_keysearch) || '%'
            OR e.event_name ILIKE '%' || trim(p_keysearch) || '%'
            OR coalesce(e.description, '') ILIKE '%' || trim(p_keysearch) || '%'
            OR coalesce(v.venue_name, '') ILIKE '%' || trim(p_keysearch) || '%'
            OR coalesce(v.city, '') ILIKE '%' || trim(p_keysearch) || '%'
          )
      AND (
            trim(coalesce(p_status, '')) = ''
            OR e.status = p_status
          )
      AND (
            p_venue_id = -1
            OR e.venue_id = p_venue_id
          )
      AND (
            p_is_featured IS NULL
            OR e.is_featured = p_is_featured
          )
      AND (
            p_is_trending IS NULL
            OR e.is_trending = p_is_trending
          )
      AND (
            p_from_date IS NULL
            OR e.end_time >= p_from_date
          )
      AND (
            p_to_date IS NULL
            OR e.start_time <= p_to_date
          )
    ORDER BY
        e.is_featured DESC,
        e.is_trending DESC,
        e.display_order ASC,
        e.start_time ASC,
        e.event_id DESC
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: event_seat_inventory_delete(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_seat_inventory_delete(p_event_seat_inventory_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_seat_inventory_delete(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_seat_inventory_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    DELETE FROM ticketing.event_seat_inventory
    WHERE event_seat_inventory_id = p_event_seat_inventory_id;

    OPEN v_out FOR
    SELECT
        p_event_seat_inventory_id AS event_seat_inventory_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_seat_inventory_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_seat_inventory_getbyid(p_event_seat_inventory_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_seat_inventory_getbyid(1);
    FETCH ALL IN <cursor_name>;
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_seat_inventory_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        esi.event_seat_inventory_id,
        esi.event_id,
        e.event_code,
        e.event_name,
        esi.seat_id,
        vs.seat_code,
        vs.row_label,
        vs.seat_number,
        vs.seat_label,
        esi.event_zone_id,
        ez.zone_code,
        ez.zone_name,
        esi.seat_status,
        esi.current_hold_id,
        sh.hold_code,
        esi.current_order_item_id,
        NULL::varchar AS order_item_code,
        esi.base_price,
        esi.version,
        esi.updated_at
    FROM ticketing.event_seat_inventory esi
    LEFT JOIN ticketing."event" e
        ON e.event_id = esi.event_id
    LEFT JOIN ticketing.venue_seat vs
        ON vs.seat_id = esi.seat_id
    LEFT JOIN ticketing.event_zone ez
        ON ez.event_zone_id = esi.event_zone_id
    LEFT JOIN ticketing.seat_hold sh
        ON sh.hold_id = esi.current_hold_id
    LEFT JOIN ticketing.ticket_order_item toi
        ON toi.order_item_id = esi.current_order_item_id
    WHERE esi.event_seat_inventory_id = p_event_seat_inventory_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_seat_inventory_getbyseatids(bigint, bigint[]); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_seat_inventory_getbyseatids(p_event_id bigint, p_seat_ids bigint[]) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_seat_inventory_getbyseatids(
        p_event_id => 5,
        p_seat_ids => ARRAY[16101, 16102, 17101]
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_seat_inventory_getbyseatids_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        esi.event_seat_inventory_id,
        esi.event_id,
        e.event_code,
        e.event_name,

        esi.seat_id,
        s.section_id,
        s.seat_code,
        s.row_label,
        s.seat_number,
        s.seat_label,
        s.x_pos,
        s.y_pos,
        s.seat_type,
        s.status AS venue_seat_status,

        esi.event_zone_id,
        ez.zone_code,
        ez.zone_name,
        ez.color_hex,

        esi.seat_status,
        esi.current_hold_id,
        sh.hold_code,
        sh.status AS hold_status,
        sh.hold_started_at,
        sh.hold_expires_at,

        esi.current_order_item_id,
        toi.order_id,
        toi.item_status,

        esi.base_price,
        esi."version",
        esi.updated_at
    FROM ticketing.event_seat_inventory esi
    INNER JOIN ticketing."event" e
        ON e.event_id = esi.event_id
    INNER JOIN ticketing.venue_seat s
        ON s.seat_id = esi.seat_id
    INNER JOIN ticketing.event_zone ez
        ON ez.event_zone_id = esi.event_zone_id
    LEFT JOIN ticketing.seat_hold sh
        ON sh.hold_id = esi.current_hold_id
    LEFT JOIN ticketing.ticket_order_item toi
        ON toi.order_item_id = esi.current_order_item_id
    WHERE esi.event_id = p_event_id
      AND (
            p_seat_ids IS NULL
            OR cardinality(p_seat_ids) = 0
            OR esi.seat_id = ANY(p_seat_ids)
          )
    ORDER BY
        ez.display_order ASC,
        s.section_id ASC,
        s.row_label ASC,
        s.seat_number ASC,
        esi.seat_id ASC;

    RETURN v_out;
END;
$$;


--
-- Name: event_seat_inventory_getpagedlist(integer, integer, character varying, bigint, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_seat_inventory_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_event_id bigint, p_event_zone_id bigint, p_seat_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_seat_inventory_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_event_id => -1,
        p_event_zone_id => -1,
        p_seat_status => ''
    );
    FETCH ALL IN <cursor_name>;
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_seat_inventory_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY esi.updated_at DESC, esi.event_seat_inventory_id DESC
            ) AS row_index,
            COUNT(*) OVER () AS row_total,

            esi.event_seat_inventory_id,
            esi.event_id,
            e.event_code,
            e.event_name,
            esi.seat_id,
            vs.seat_code,
            vs.row_label,
            vs.seat_number,
            vs.seat_label,
            esi.event_zone_id,
            ez.zone_code,
            ez.zone_name,
            esi.seat_status,
            esi.current_hold_id,
            sh.hold_code,
            esi.current_order_item_id,
            NULL::varchar AS order_item_code,
            esi.base_price,
            esi.version,
            esi.updated_at
        FROM ticketing.event_seat_inventory esi
        LEFT JOIN ticketing."event" e
            ON e.event_id = esi.event_id
        LEFT JOIN ticketing.venue_seat vs
            ON vs.seat_id = esi.seat_id
        LEFT JOIN ticketing.event_zone ez
            ON ez.event_zone_id = esi.event_zone_id
        LEFT JOIN ticketing.seat_hold sh
            ON sh.hold_id = esi.current_hold_id
        LEFT JOIN ticketing.ticket_order_item toi
            ON toi.order_item_id = esi.current_order_item_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(vs.seat_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(vs.seat_label, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ez.zone_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ez.zone_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_event_id = -1
                OR esi.event_id = p_event_id
              )
          AND (
                p_event_zone_id = -1
                OR esi.event_zone_id = p_event_zone_id
              )
          AND (
                trim(coalesce(p_seat_status, '')) = ''
                OR esi.seat_status = p_seat_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: event_seat_inventory_insert(bigint, bigint, bigint, character varying, bigint, bigint, numeric, integer); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_seat_inventory_insert(p_event_id bigint, p_seat_id bigint, p_event_zone_id bigint, p_seat_status character varying, p_current_hold_id bigint, p_current_order_item_id bigint, p_base_price numeric, p_version integer) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_seat_inventory_insert(
        p_event_id => 1,
        p_seat_id => 101,
        p_event_zone_id => 1,
        p_seat_status => 'available',
        p_current_hold_id => NULL,
        p_current_order_item_id => NULL,
        p_base_price => 500000,
        p_version => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_seat_inventory_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_event_seat_inventory_id bigint;
BEGIN
    INSERT INTO ticketing.event_seat_inventory
    (
        event_id,
        seat_id,
        event_zone_id,
        seat_status,
        current_hold_id,
        current_order_item_id,
        base_price,
        version,
        updated_at
    )
    VALUES
    (
        p_event_id,
        p_seat_id,
        p_event_zone_id,
        p_seat_status,
        p_current_hold_id,
        p_current_order_item_id,
        p_base_price,
        p_version,
        now()
    )
    RETURNING event_seat_inventory_id INTO v_event_seat_inventory_id;

    OPEN v_out FOR
    SELECT
        v_event_seat_inventory_id AS event_seat_inventory_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_seat_inventory_update(bigint, bigint, bigint, bigint, character varying, bigint, bigint, numeric, integer); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_seat_inventory_update(p_event_seat_inventory_id bigint, p_event_id bigint, p_seat_id bigint, p_event_zone_id bigint, p_seat_status character varying, p_current_hold_id bigint, p_current_order_item_id bigint, p_base_price numeric, p_version integer) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_seat_inventory_update(
        p_event_seat_inventory_id => 1,
        p_event_id => 1,
        p_seat_id => 101,
        p_event_zone_id => 1,
        p_seat_status => 'held',
        p_current_hold_id => 10,
        p_current_order_item_id => NULL,
        p_base_price => 500000,
        p_version => 2
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_seat_inventory_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.event_seat_inventory
    SET
        event_id = p_event_id,
        seat_id = p_seat_id,
        event_zone_id = p_event_zone_id,
        seat_status = p_seat_status,
        current_hold_id = p_current_hold_id,
        current_order_item_id = p_current_order_item_id,
        base_price = p_base_price,
        version = p_version,
        updated_at = now()
    WHERE event_seat_inventory_id = p_event_seat_inventory_id;

    OPEN v_out FOR
    SELECT
        p_event_seat_inventory_id AS event_seat_inventory_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_seat_inventory_update_hold(bigint, bigint, bigint, bigint, character varying, bigint, numeric, integer); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_seat_inventory_update_hold(p_event_seat_inventory_id bigint, p_event_id bigint, p_seat_id bigint, p_event_zone_id bigint, p_seat_status character varying, p_current_hold_id bigint, p_base_price numeric, p_version integer) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_seat_inventory_update(
        p_event_seat_inventory_id => 1,
        p_event_id => 1,
        p_seat_id => 101,
        p_event_zone_id => 1,
        p_seat_status => 'held',
        p_current_hold_id => 10,
        p_current_order_item_id => NULL,
        p_base_price => 500000,
        p_version => 2
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_seat_inventory_update_hold_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.event_seat_inventory
    SET
        event_id = p_event_id,
        seat_id = p_seat_id,
        event_zone_id = p_event_zone_id,
        seat_status = p_seat_status,
        current_hold_id = p_current_hold_id,
        base_price = p_base_price,
        version = p_version,
        updated_at = now()
    WHERE event_seat_inventory_id = p_event_seat_inventory_id;

    OPEN v_out FOR
    SELECT
        p_event_seat_inventory_id AS event_seat_inventory_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_seat_inventory_update_release(bigint, bigint, bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_seat_inventory_update_release(p_event_seat_inventory_id bigint, p_event_id bigint, p_seat_id bigint, p_event_zone_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'event_seat_inventory_update_release_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.event_seat_inventory
    SET
        seat_status     = 'available',
        current_hold_id = NULL,
        version         = version + 1,
        updated_at      = NOW()
    WHERE event_seat_inventory_id = p_event_seat_inventory_id
      AND event_id                = p_event_id
      AND seat_id                 = p_seat_id
      AND event_zone_id           = p_event_zone_id
      AND seat_status             = 'held';

    OPEN v_out FOR
        SELECT p_event_seat_inventory_id AS event_seat_inventory_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_seat_inventory_updateorder(bigint, bigint, bigint, bigint, character varying, bigint, numeric, integer); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_seat_inventory_updateorder(p_event_seat_inventory_id bigint, p_event_id bigint, p_seat_id bigint, p_event_zone_id bigint, p_seat_status character varying, p_current_order_item_id bigint, p_base_price numeric, p_version integer) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_seat_inventory_update(
        p_event_seat_inventory_id => 1,
        p_event_id => 1,
        p_seat_id => 101,
        p_event_zone_id => 1,
        p_seat_status => 'held',
        p_current_hold_id => 10,
        p_current_order_item_id => NULL,
        p_base_price => 500000,
        p_version => 2
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_seat_inventory_updateorder_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.event_seat_inventory
    SET
        event_id = p_event_id,
        seat_id = p_seat_id,
        event_zone_id = p_event_zone_id,
        seat_status = p_seat_status,
        current_order_item_id = p_current_order_item_id,
        base_price = p_base_price,
        version = p_version,
        updated_at = now()
    WHERE event_seat_inventory_id = p_event_seat_inventory_id;

    OPEN v_out FOR
    SELECT
        p_event_seat_inventory_id AS event_seat_inventory_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_update(bigint, character varying, character varying, text, bigint, character varying, timestamp without time zone, timestamp without time zone, timestamp without time zone, timestamp without time zone, character varying, timestamp without time zone, timestamp without time zone, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_update(p_event_id bigint, p_event_code character varying, p_event_name character varying, p_description text, p_venue_id bigint, p_banner_url character varying, p_start_time timestamp without time zone, p_end_time timestamp without time zone, p_sale_start_time timestamp without time zone, p_sale_end_time timestamp without time zone, p_status character varying, p_published_at timestamp without time zone, p_on_sale_at timestamp without time zone, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_update(
        p_event_id => 1,
        p_event_code => 'EVT_QK7_001',
        p_event_name => 'Sky Tour Live Concert 2026 - TP.HCM Updated',
        p_description => 'Updated event',
        p_venue_id => 13,
        p_banner_url => 'test-updated',
        p_start_time => '2026-03-28 11:03:00'::timestamp,
        p_end_time => '2026-03-29 11:03:00'::timestamp,
        p_sale_start_time => '2026-03-24 11:03:00'::timestamp,
        p_sale_end_time => '2026-03-27 11:03:00'::timestamp,
        p_status => 'draft',
        p_published_at => '2026-03-23 11:03:00'::timestamp,
        p_on_sale_at => '2026-03-24 11:03:00'::timestamp,
        p_updated_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing."event"
    SET
        event_code = trim(p_event_code),
        event_name = trim(p_event_name),
        description = p_description,
        venue_id = p_venue_id,
        banner_url = p_banner_url,
        start_time = p_start_time,
        end_time = p_end_time,
        sale_start_time = p_sale_start_time,
        sale_end_time = p_sale_end_time,
        status = p_status,
        published_at = p_published_at,
        on_sale_at = p_on_sale_at,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE event_id = p_event_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_event_id AS event_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_update(bigint, character varying, character varying, text, bigint, character varying, timestamp without time zone, timestamp without time zone, timestamp without time zone, timestamp without time zone, character varying, timestamp without time zone, timestamp without time zone, boolean, boolean, integer, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_update(p_event_id bigint, p_event_code character varying, p_event_name character varying, p_description text, p_venue_id bigint, p_banner_url character varying, p_start_time timestamp without time zone, p_end_time timestamp without time zone, p_sale_start_time timestamp without time zone, p_sale_end_time timestamp without time zone, p_status character varying, p_published_at timestamp without time zone, p_on_sale_at timestamp without time zone, p_is_featured boolean, p_is_trending boolean, p_display_order integer, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_update(
        p_event_id => 1,
        p_event_code => 'EVT001',
        p_event_name => 'Sky Tour Live Concert 2026 - TP.HCM Updated',
        p_description => 'Updated event',
        p_venue_id => 1,
        p_banner_url => 'https://placehold.co/800x400/png',
        p_start_time => '2026-06-20 19:30:00'::timestamp,
        p_end_time => '2026-06-20 22:30:00'::timestamp,
        p_sale_start_time => '2026-05-15 10:00:00'::timestamp,
        p_sale_end_time => '2026-06-20 18:30:00'::timestamp,
        p_status => 'on_sale',
        p_published_at => '2026-05-10 09:00:00'::timestamp,
        p_on_sale_at => '2026-05-15 10:00:00'::timestamp,
        p_is_featured => true,
        p_is_trending => false,
        p_display_order => 2,
        p_updated_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing."event"
    SET
        event_code = trim(p_event_code),
        event_name = trim(p_event_name),
        description = p_description,
        venue_id = p_venue_id,
        banner_url = p_banner_url,
        start_time = p_start_time,
        end_time = p_end_time,
        sale_start_time = p_sale_start_time,
        sale_end_time = p_sale_end_time,
        status = p_status,
        published_at = p_published_at,
        on_sale_at = p_on_sale_at,
        is_featured = COALESCE(p_is_featured, false),
        is_trending = COALESCE(p_is_trending, false),
        display_order = COALESCE(p_display_order, 0),
        updated_by = p_updated_by,
        updated_at = now()
    WHERE event_id = p_event_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_event_id AS event_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_delete(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_delete(p_event_zone_id bigint, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_delete(1, 1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.event_zone
    SET
        is_deleted = true,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE event_zone_id = p_event_zone_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_event_zone_id AS event_zone_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_getbyeventid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_getbyeventid(p_event_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_getbyeventid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_getbyeventid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        ez.event_zone_id,
        ez.event_id,
        e.event_code,
        e.event_name,
        ez.zone_code,
        ez.zone_name,
        ez.color_hex,
        ez.description,
        ez.display_order,
        ez.status,
        ez.created_by,
        uc.full_name AS created_by_name,
        ez.created_at,
        ez.updated_by,
        uu.full_name AS updated_by_name,
        ez.updated_at,
        ez.is_deleted
    FROM ticketing.event_zone ez
    LEFT JOIN ticketing."event" e
        ON e.event_id = ez.event_id
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = ez.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = ez.updated_by
    WHERE ez.event_id = p_event_id
      AND ez.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_getbyid(p_event_zone_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        ez.event_zone_id,
        ez.event_id,
        e.event_code,
        e.event_name,
        ez.zone_code,
        ez.zone_name,
        ez.color_hex,
        ez.description,
        ez.display_order,
        ez.status,
        ez.created_by,
        uc.full_name AS created_by_name,
        ez.created_at,
        ez.updated_by,
        uu.full_name AS updated_by_name,
        ez.updated_at,
        ez.is_deleted
    FROM ticketing.event_zone ez
    LEFT JOIN ticketing."event" e
        ON e.event_id = ez.event_id
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = ez.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = ez.updated_by
    WHERE ez.event_zone_id = p_event_zone_id
      AND ez.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_getpagedlist(integer, integer, character varying, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_event_id bigint, p_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_event_id => -1,
        p_status => ''
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY ez.display_order ASC, ez.created_at DESC, ez.event_zone_id DESC) AS row_index,
            COUNT(*) OVER () AS row_total,

            ez.event_zone_id,
            ez.event_id,
            e.event_code,
            e.event_name,
            ez.zone_code,
            ez.zone_name,
            ez.color_hex,
            ez.description,
            ez.display_order,
            ez.status,
            ez.created_by,
            uc.full_name AS created_by_name,
            ez.created_at,
            ez.updated_by,
            uu.full_name AS updated_by_name,
            ez.updated_at
        FROM ticketing.event_zone ez
        LEFT JOIN ticketing."event" e
            ON e.event_id = ez.event_id
        LEFT JOIN ticketing.sys_user uc
            ON uc.user_id = ez.created_by
        LEFT JOIN ticketing.sys_user uu
            ON uu.user_id = ez.updated_by
        WHERE ez.is_deleted = false
          AND (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(ez.zone_code) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(ez.zone_name) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ez.description, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_event_id = -1
                OR ez.event_id = p_event_id
              )
          AND (
                trim(coalesce(p_status, '')) = ''
                OR ez.status = p_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_insert(bigint, character varying, character varying, character varying, character varying, integer, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_insert(p_event_id bigint, p_zone_code character varying, p_zone_name character varying, p_color_hex character varying, p_description character varying, p_display_order integer, p_status character varying, p_created_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_insert(
        p_event_id => 1,
        p_zone_code => 'VIP',
        p_zone_name => 'Khu VIP',
        p_color_hex => '#FF0000',
        p_description => 'Zone VIP',
        p_display_order => 1,
        p_status => 'active',
        p_created_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_event_zone_id bigint;
BEGIN
    INSERT INTO ticketing.event_zone
    (
        event_id,
        zone_code,
        zone_name,
        color_hex,
        description,
        display_order,
        status,
        created_by,
        created_at,
        is_deleted
    )
    VALUES
    (
        p_event_id,
        trim(p_zone_code),
        trim(p_zone_name),
        p_color_hex,
        p_description,
        p_display_order,
        p_status,
        p_created_by,
        now(),
        false
    )
    RETURNING event_zone_id INTO v_event_zone_id;

    OPEN v_out FOR
    SELECT v_event_zone_id AS event_zone_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_price_delete(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_price_delete(p_event_zone_price_id bigint, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_price_delete(1, 1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_price_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.event_zone_price
    SET
        is_deleted = true,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE event_zone_price_id = p_event_zone_price_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT
        p_event_zone_price_id AS event_zone_price_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_price_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_price_getbyid(p_event_zone_price_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_price_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_price_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        ezp.event_zone_price_id,
        ezp.event_zone_id,
        ez.event_id,
        e.event_code,
        e.event_name,
        ez.zone_code,
        ez.zone_name,
        ezp.price,
        ezp.currency,
        ezp.start_time,
        ezp.end_time,
        ezp.is_active,
        ezp.created_by,
        uc.full_name AS created_by_name,
        ezp.created_at,
        ezp.updated_by,
        uu.full_name AS updated_by_name,
        ezp.updated_at,
        ezp.is_deleted
    FROM ticketing.event_zone_price ezp
    LEFT JOIN ticketing.event_zone ez
        ON ez.event_zone_id = ezp.event_zone_id
    LEFT JOIN ticketing."event" e
        ON e.event_id = ez.event_id
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = ezp.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = ezp.updated_by
    WHERE ezp.event_zone_price_id = p_event_zone_price_id
      AND ezp.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_price_getbyzoneid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_price_getbyzoneid(p_event_zone_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_price_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_price_getbyzoneid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        ezp.event_zone_price_id,
        ezp.event_zone_id,
        ez.event_id,
        e.event_code,
        e.event_name,
        ez.zone_code,
        ez.zone_name,
        ezp.price,
        ezp.currency,
        ezp.start_time,
        ezp.end_time,
        ezp.is_active,
        ezp.created_by,
        uc.full_name AS created_by_name,
        ezp.created_at,
        ezp.updated_by,
        uu.full_name AS updated_by_name,
        ezp.updated_at,
        ezp.is_deleted
    FROM ticketing.event_zone_price ezp
    LEFT JOIN ticketing.event_zone ez
        ON ez.event_zone_id = ezp.event_zone_id
    LEFT JOIN ticketing."event" e
        ON e.event_id = ez.event_id
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = ezp.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = ezp.updated_by
    WHERE ezp.event_zone_id = p_event_zone_id
      AND ezp.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_price_getbyzoneids(bigint[]); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_price_getbyzoneids(p_event_zone_ids bigint[]) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'event_zone_price_getbyzoneids_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM ticketing.event_zone_price
    WHERE event_zone_id = ANY(p_event_zone_ids)
      AND is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_price_getpagedlist(integer, integer, character varying, bigint, boolean); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_price_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_event_zone_id bigint, p_is_active boolean) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_price_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_event_zone_id => -1,
        p_is_active => true
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_price_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY ezp.created_at DESC, ezp.event_zone_price_id DESC) AS row_index,
            COUNT(*) OVER () AS row_total,

            ezp.event_zone_price_id,
            ezp.event_zone_id,
            ez.event_id,
            e.event_code,
            e.event_name,
            ez.zone_code,
            ez.zone_name,
            ezp.price,
            ezp.currency,
            ezp.start_time,
            ezp.end_time,
            ezp.is_active,
            ezp.created_by,
            uc.full_name AS created_by_name,
            ezp.created_at,
            ezp.updated_by,
            uu.full_name AS updated_by_name,
            ezp.updated_at
        FROM ticketing.event_zone_price ezp
        LEFT JOIN ticketing.event_zone ez
            ON ez.event_zone_id = ezp.event_zone_id
        LEFT JOIN ticketing."event" e
            ON e.event_id = ez.event_id
        LEFT JOIN ticketing.sys_user uc
            ON uc.user_id = ezp.created_by
        LEFT JOIN ticketing.sys_user uu
            ON uu.user_id = ezp.updated_by
        WHERE ezp.is_deleted = false
          AND (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ez.zone_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ez.zone_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ezp.currency, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_event_zone_id = -1
                OR ezp.event_zone_id = p_event_zone_id
              )
          AND (
                ezp.is_active = p_is_active
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_price_insert(bigint, numeric, character varying, timestamp without time zone, timestamp without time zone, boolean, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_price_insert(p_event_zone_id bigint, p_price numeric, p_currency character varying, p_start_time timestamp without time zone, p_end_time timestamp without time zone, p_is_active boolean, p_created_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_price_insert(
        p_event_zone_id => 1,
        p_price => 500000,
        p_currency => 'VND',
        p_start_time => NULL,
        p_end_time => NULL,
        p_is_active => true,
        p_created_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_price_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_event_zone_price_id bigint;
BEGIN
    INSERT INTO ticketing.event_zone_price
    (
        event_zone_id,
        price,
        currency,
        start_time,
        end_time,
        is_active,
        created_by,
        created_at,
        is_deleted
    )
    VALUES
    (
        p_event_zone_id,
        p_price,
        p_currency,
        p_start_time,
        p_end_time,
        p_is_active,
        p_created_by,
        now(),
        false
    )
    RETURNING event_zone_price_id INTO v_event_zone_price_id;

    OPEN v_out FOR
    SELECT
        v_event_zone_price_id AS event_zone_price_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_price_update(bigint, bigint, numeric, character varying, timestamp without time zone, timestamp without time zone, boolean, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_price_update(p_event_zone_price_id bigint, p_event_zone_id bigint, p_price numeric, p_currency character varying, p_start_time timestamp without time zone, p_end_time timestamp without time zone, p_is_active boolean, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_price_update(
        p_event_zone_price_id => 1,
        p_event_zone_id => 1,
        p_price => 650000,
        p_currency => 'VND',
        p_start_time => '2026-04-01 00:00:00',
        p_end_time => '2026-04-10 23:59:59',
        p_is_active => true,
        p_updated_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_price_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.event_zone_price
    SET
        event_zone_id = p_event_zone_id,
        price = p_price,
        currency = p_currency,
        start_time = p_start_time,
        end_time = p_end_time,
        is_active = p_is_active,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE event_zone_price_id = p_event_zone_price_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT
        p_event_zone_price_id AS event_zone_price_id;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_section_delete(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_section_delete(p_event_zone_section_id bigint, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_delete(1, 1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_delete_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: event_zone_section_getbyeventzoneid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_section_getbyeventzoneid(p_event_zone_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_getbyeventzoneid_' || replace(gen_random_uuid()::text, '-', '_');
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
    WHERE ezs.event_zone_id = p_event_zone_id
      AND ezs.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_section_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_section_getbyid(p_event_zone_section_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: event_zone_section_geteventid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_section_geteventid(p_event_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_geteventid_' || replace(gen_random_uuid()::text, '-', '_');
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
    WHERE ezs.event_id = p_event_id
      AND ezs.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: event_zone_section_getpagedlist(integer, integer, bigint, bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_section_getpagedlist(p_pagesize integer, p_offset integer, p_event_id bigint, p_event_zone_id bigint, p_section_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_event_id => -1,
        p_event_zone_id => -1,
        p_section_id => -1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY
                    ez.display_order ASC,
                    vs.display_order ASC,
                    ezs.created_at DESC,
                    ezs.event_zone_section_id DESC
            ) AS row_index,
            COUNT(*) OVER () AS row_total,

            ezs.event_zone_section_id,
            ezs.event_id,
            e.event_code,
            e.event_name,
            ezs.event_zone_id,
            ez.zone_code,
            ez.zone_name,
            ez.display_order AS zone_display_order,
            ezs.section_id,
            vs.section_code,
            vs.section_name,
            vs.display_order AS section_display_order,
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
$$;


--
-- Name: event_zone_section_insert(bigint, bigint, bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_section_insert(p_event_id bigint, p_event_zone_id bigint, p_section_id bigint, p_created_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_insert(
        p_event_id => 5,
        p_event_zone_id => 17,
        p_section_id => 17,
        p_created_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_insert_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: event_zone_section_update(bigint, bigint, bigint, bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_section_update(p_event_zone_section_id bigint, p_event_id bigint, p_event_zone_id bigint, p_section_id bigint, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_section_update(
        p_event_zone_section_id => 1,
        p_event_id => 5,
        p_event_zone_id => 17,
        p_section_id => 18,
        p_updated_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_section_update_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: event_zone_update(bigint, bigint, character varying, character varying, character varying, character varying, integer, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.event_zone_update(p_event_zone_id bigint, p_event_id bigint, p_zone_code character varying, p_zone_name character varying, p_color_hex character varying, p_description character varying, p_display_order integer, p_status character varying, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.event_zone_update(
        p_event_zone_id => 1,
        p_event_id => 1,
        p_zone_code => 'VIP',
        p_zone_name => 'Khu VIP Updated',
        p_color_hex => '#00FF00',
        p_description => 'Updated zone',
        p_display_order => 2,
        p_status => 'active',
        p_updated_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'event_zone_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.event_zone
    SET
        event_id = p_event_id,
        zone_code = trim(p_zone_code),
        zone_name = trim(p_zone_name),
        color_hex = p_color_hex,
        description = p_description,
        display_order = p_display_order,
        status = p_status,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE event_zone_id = p_event_zone_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_event_zone_id AS event_zone_id;

    RETURN v_out;
END;
$$;


--
-- Name: idempotency_request_check(bigint, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.idempotency_request_check(p_idempotency_id bigint DEFAULT 0, p_idempotency_key character varying DEFAULT ''::character varying, p_request_type character varying DEFAULT ''::character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'idempotency_request_check_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: idempotency_request_delete(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.idempotency_request_delete(p_idempotency_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.idempotency_request_delete(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'idempotency_request_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    DELETE FROM ticketing.idempotency_request
    WHERE idempotency_id = p_idempotency_id;

    OPEN v_out FOR
    SELECT p_idempotency_id AS idempotency_id;

    RETURN v_out;
END;
$$;


--
-- Name: idempotency_request_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.idempotency_request_getbyid(p_idempotency_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'idempotency_request_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        ir.idempotency_id,
        ir.idempotency_key,
        ir.request_type,
        ir.customer_id,
        c.username  AS customer_username,
        c.full_name AS customer_full_name,
        c.email     AS customer_email,
        c.phone     AS customer_phone,
        ir.request_hash,
        ir.status,
        ir.response_snapshot,
        ir.created_at,
        ir.expired_at
    FROM ticketing.idempotency_request ir
    LEFT JOIN ticketing.customer c
        ON c.customer_id = ir.customer_id
    WHERE ir.idempotency_id = p_idempotency_id;

    RETURN v_out;
END;
$$;


--
-- Name: idempotency_request_getbyidempotency_key(character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.idempotency_request_getbyidempotency_key(p_idempotency_key character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'idempotency_request_getbyidempotency_key_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        ir.idempotency_id,
        ir.idempotency_key,
        ir.request_type,
        ir.customer_id,
        c.username  AS customer_username,
        c.full_name AS customer_full_name,
        c.email     AS customer_email,
        c.phone     AS customer_phone,
        ir.request_hash,
        ir.status,
        ir.response_snapshot,
        ir.created_at,
        ir.expired_at
    FROM ticketing.idempotency_request ir
    LEFT JOIN ticketing.customer c
        ON c.customer_id = ir.customer_id
    WHERE ir.idempotency_key = p_idempotency_key;

    RETURN v_out;
END;
$$;


--
-- Name: idempotency_request_getbykeyandtype(character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.idempotency_request_getbykeyandtype(p_idempotency_key character varying, p_request_type character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'idempotency_request_getbykeyandtype_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        ir.idempotency_id,
        ir.idempotency_key,
        ir.request_type,
        ir.customer_id,
        c.username  AS customer_username,
        c.full_name AS customer_full_name,
        c.email     AS customer_email,
        c.phone     AS customer_phone,
        ir.request_hash,
        ir.status,
        ir.response_snapshot,
        ir.created_at,
        ir.expired_at
    FROM ticketing.idempotency_request ir
    LEFT JOIN ticketing.customer c
        ON c.customer_id = ir.customer_id
    WHERE ir.idempotency_key = p_idempotency_key
      AND ir.request_type = p_request_type;

    RETURN v_out;
END;
$$;


--
-- Name: idempotency_request_getpagedlist(integer, integer, character varying, bigint, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.idempotency_request_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_customer_id bigint, p_request_type character varying, p_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'idempotency_request_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
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
            ir.customer_id,
            c.username  AS customer_username,
            c.full_name AS customer_full_name,
            c.email     AS customer_email,
            c.phone     AS customer_phone,
            ir.request_hash,
            ir.status,
            ir.response_snapshot,
            ir.created_at,
            ir.expired_at
        FROM ticketing.idempotency_request ir
        LEFT JOIN ticketing.customer c
            ON c.customer_id = ir.customer_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(ir.idempotency_key) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ir.request_hash, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.username, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.email, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.phone, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_customer_id = -1
                OR ir.customer_id = p_customer_id
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
$$;


--
-- Name: idempotency_request_insert(character varying, character varying, bigint, character varying, character varying, text, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.idempotency_request_insert(p_idempotency_key character varying, p_request_type character varying, p_customer_id bigint, p_request_hash character varying, p_status character varying, p_response_snapshot text, p_expired_at timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'idempotency_request_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_idempotency_id bigint;
BEGIN
    INSERT INTO ticketing.idempotency_request
    (
        idempotency_key,
        request_type,
        customer_id,
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
        p_customer_id,
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
$$;


--
-- Name: idempotency_request_update(bigint, character varying, character varying, bigint, character varying, character varying, text, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.idempotency_request_update(p_idempotency_id bigint, p_idempotency_key character varying, p_request_type character varying, p_customer_id bigint, p_request_hash character varying, p_status character varying, p_response_snapshot text, p_expired_at timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'idempotency_request_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.idempotency_request
    SET
        idempotency_key = trim(p_idempotency_key),
        request_type = trim(p_request_type),
        customer_id = p_customer_id,
        request_hash = p_request_hash,
        status = p_status,
        response_snapshot = p_response_snapshot,
        expired_at = p_expired_at
    WHERE idempotency_id = p_idempotency_id;

    OPEN v_out FOR
    SELECT p_idempotency_id AS idempotency_id;

    RETURN v_out;
END;
$$;


--
-- Name: payment_callback_log_delete(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_callback_log_delete(p_callback_log_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: payment_callback_log_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_callback_log_getbyid(p_callback_log_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: payment_callback_log_getbypaymentid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_callback_log_getbypaymentid(p_payment_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: payment_callback_log_getpagedlist(integer, integer, character varying, bigint, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_callback_log_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_payment_id bigint, p_payment_provider character varying, p_processed_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: payment_callback_log_insert(bigint, character varying, character varying, character varying, text, boolean, character varying, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_callback_log_insert(p_payment_id bigint, p_payment_provider character varying, p_external_transaction_ref character varying, p_callback_signature character varying, p_payload text, p_signature_valid boolean, p_processed_status character varying, p_received_at timestamp without time zone, p_processed_at timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: payment_callback_log_update(bigint, bigint, character varying, character varying, character varying, text, boolean, character varying, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_callback_log_update(p_callback_log_id bigint, p_payment_id bigint, p_payment_provider character varying, p_external_transaction_ref character varying, p_callback_signature character varying, p_payload text, p_signature_valid boolean, p_processed_status character varying, p_received_at timestamp without time zone, p_processed_at timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: payment_transaction_check(bigint, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_transaction_check(p_payment_id bigint DEFAULT 0, p_payment_ref character varying DEFAULT ''::character varying, p_provider_transaction_ref character varying DEFAULT ''::character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: payment_transaction_delete(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_transaction_delete(p_payment_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: payment_transaction_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_transaction_getbyid(p_payment_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
        o.customer_id,
        c.email,
        c.full_name,

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
    LEFT JOIN ticketing.customer c
        ON c.customer_id = o.customer_id
    WHERE p.payment_id = p_payment_id;

    RETURN v_out;
END;
$$;


--
-- Name: payment_transaction_getbyorderid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_transaction_getbyorderid(p_order_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
        o.customer_id,
        c.email,
        c.full_name,

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
    LEFT JOIN ticketing.customer c
        ON c.customer_id = o.customer_id
    WHERE p.order_id = p_order_id;

    RETURN v_out;
END;
$$;


--
-- Name: payment_transaction_getpagedlist(integer, integer, character varying, bigint, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_transaction_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_order_id bigint, p_payment_provider character varying, p_payment_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
            o.customer_id,
            c.email,
            c.full_name,

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
        LEFT JOIN ticketing.customer c
            ON c.customer_id = o.customer_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(p.payment_ref) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(p.provider_transaction_ref, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(o.order_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.username, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
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
$$;


--
-- Name: payment_transaction_insert(bigint, character varying, character varying, character varying, numeric, character varying, timestamp without time zone, timestamp without time zone, text, text); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_transaction_insert(p_order_id bigint, p_payment_provider character varying, p_payment_ref character varying, p_provider_transaction_ref character varying, p_amount numeric, p_payment_status character varying, p_requested_at timestamp without time zone, p_confirmed_at timestamp without time zone, p_raw_request_payload text, p_raw_callback_payload text) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: payment_transaction_update(bigint, bigint, character varying, character varying, character varying, numeric, character varying, timestamp without time zone, timestamp without time zone, text, text); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.payment_transaction_update(p_payment_id bigint, p_order_id bigint, p_payment_provider character varying, p_payment_ref character varying, p_provider_transaction_ref character varying, p_amount numeric, p_payment_status character varying, p_requested_at timestamp without time zone, p_confirmed_at timestamp without time zone, p_raw_request_payload text, p_raw_callback_payload text) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- Name: seat_hold_check(bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_check(p_hold_id bigint DEFAULT 0, p_hold_code character varying DEFAULT ''::character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'seat_hold_check_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_delete(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_delete(p_hold_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.seat_hold_delete(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    DELETE FROM ticketing.seat_hold
    WHERE hold_id = p_hold_id;

    OPEN v_out FOR
    SELECT p_hold_id AS hold_id;

    RETURN v_out;
END;
$$;


--
-- Name: seat_hold_getbycustomerid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_getbycustomerid(p_customer_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.seat_hold_getbycustomerid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_getbycustomerid_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_getbyeventid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_getbyeventid(p_event_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.seat_hold_getbyeventid(5);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_getbyeventid_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_getbyid(p_hold_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.seat_hold_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_getexpiredactive(); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_getexpiredactive() RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'seat_hold_getexpiredactive_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
        SELECT hold_id
        FROM   ticketing.seat_hold
        WHERE  status = 'active'
          AND  hold_expires_at < NOW();

    RETURN v_out;
END;
$$;


--
-- Name: seat_hold_getpagedlist(integer, integer, character varying, bigint, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_event_id bigint, p_customer_id bigint, p_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_insert(character varying, bigint, bigint, character varying, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_insert(p_hold_code character varying, p_event_id bigint, p_customer_id bigint, p_status character varying, p_hold_started_at timestamp without time zone, p_hold_expires_at timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.seat_hold_insert(
        p_hold_code => 'HOLD0001',
        p_event_id => 5,
        p_customer_id => 1,
        p_status => 'active',
        p_hold_started_at => now()::timestamp,
        p_hold_expires_at => (now() + interval '10 minute')::timestamp
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_insert_' || replace(gen_random_uuid()::text, '-', '_');
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
        now()
    )
    RETURNING hold_id INTO v_hold_id;

    OPEN v_out FOR
    SELECT v_hold_id AS hold_id;

    RETURN v_out;
END;
$$;


--
-- Name: seat_hold_insert(character varying, bigint, bigint, character varying, timestamp without time zone, timestamp without time zone, timestamp without time zone, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_insert(p_hold_code character varying, p_event_id bigint, p_customer_id bigint, p_status character varying, p_hold_started_at timestamp without time zone, p_hold_expires_at timestamp without time zone, p_released_at timestamp without time zone, p_release_reason character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_insert_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_item_check(bigint, bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_item_check(p_hold_item_id bigint DEFAULT 0, p_hold_id bigint DEFAULT 0, p_event_seat_inventory_id bigint DEFAULT 0) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'seat_hold_item_check_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_item_delete(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_item_delete(p_hold_item_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.seat_hold_item_delete(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    DELETE FROM ticketing.seat_hold_item
    WHERE hold_item_id = p_hold_item_id;

    OPEN v_out FOR
    SELECT p_hold_item_id AS hold_item_id;

    RETURN v_out;
END;
$$;


--
-- Name: seat_hold_item_getbyholdid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_item_getbyholdid(p_hold_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.seat_hold_item_getbyholdid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_getbyholdid_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_item_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_item_getbyid(p_hold_item_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.seat_hold_item_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_item_getpagedlist(integer, integer, bigint, bigint, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_item_getpagedlist(p_pagesize integer, p_offset integer, p_hold_id bigint, p_event_seat_inventory_id bigint, p_zone_id bigint, p_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_item_insert(bigint, bigint, bigint, bigint, numeric, character varying, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_item_insert(p_hold_id bigint, p_event_seat_inventory_id bigint, p_seat_id bigint, p_zone_id bigint, p_price_at_hold numeric, p_seat_label_snapshot character varying, p_zone_name_snapshot character varying, p_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_insert_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_item_update(bigint, bigint, bigint, bigint, bigint, numeric, character varying, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_item_update(p_hold_item_id bigint, p_hold_id bigint, p_event_seat_inventory_id bigint, p_seat_id bigint, p_zone_id bigint, p_price_at_hold numeric, p_seat_label_snapshot character varying, p_zone_name_snapshot character varying, p_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_item_update_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: seat_hold_item_updatestatusbyholdid(bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_item_updatestatusbyholdid(p_hold_id bigint, p_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'seat_hold_item_updatestatusbyholdid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.seat_hold_item
    SET    status     = p_status
           --updated_at = NOW()
    WHERE  hold_id    = p_hold_id;
      --AND  is_deleted = FALSE;

    OPEN v_out FOR
        SELECT p_hold_id AS hold_id;

    RETURN v_out;
END;
$$;


--
-- Name: seat_hold_update(bigint, character varying, bigint, bigint, character varying, timestamp without time zone, timestamp without time zone, timestamp without time zone, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.seat_hold_update(p_hold_id bigint, p_hold_code character varying, p_event_id bigint, p_customer_id bigint, p_status character varying, p_hold_started_at timestamp without time zone, p_hold_expires_at timestamp without time zone, p_released_at timestamp without time zone, p_release_reason character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
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
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'seat_hold_update_' || replace(gen_random_uuid()::text, '-', '_');
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
$$;


--
-- Name: sys_role_delete(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_role_delete(p_role_id bigint, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_role_delete(1, 1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_role_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.sys_role
    SET
        is_deleted = true,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE role_id = p_role_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_role_id AS role_id;

    RETURN v_out;
END;
$$;


--
-- Name: sys_role_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_role_getbyid(p_role_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_role_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_role_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        r.role_id,
        r.role_code,
        r.role_name,
        r.description,
        r.status,
        r.created_by,
        uc.full_name AS created_by_name,
        r.created_at,
        r.updated_by,
        uu.full_name AS updated_by_name,
        r.updated_at,
        r.is_deleted
    FROM ticketing.sys_role r
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = r.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = r.updated_by
    WHERE r.role_id = p_role_id
      AND r.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: sys_role_getpagedlist(integer, integer, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_role_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_role_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_status => ''
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_role_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY r.created_at DESC, r.role_id DESC) AS row_index,
            COUNT(*) OVER () AS row_total,
            r.role_id,
            r.role_code,
            r.role_name,
            r.description,
            r.status,
            r.created_by,
            uc.full_name AS created_by_name,
            r.created_at,
            r.updated_by,
            uu.full_name AS updated_by_name,
            r.updated_at
        FROM ticketing.sys_role r
        LEFT JOIN ticketing.sys_user uc
            ON uc.user_id = r.created_by
        LEFT JOIN ticketing.sys_user uu
            ON uu.user_id = r.updated_by
        WHERE r.is_deleted = false
          AND (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(r.role_code) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(r.role_name) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(r.description, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                trim(coalesce(p_status, '')) = ''
                OR r.status = p_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: sys_role_insert(character varying, character varying, character varying, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_role_insert(p_role_code character varying, p_role_name character varying, p_description character varying, p_status character varying, p_created_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_role_insert(
        p_role_code => 'ADMIN',
        p_role_name => 'Administrator',
        p_description => 'System administrator role',
        p_status => 'active',
        p_created_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_role_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_role_id bigint;
BEGIN
    INSERT INTO ticketing.sys_role
    (
        role_code,
        role_name,
        description,
        status,
        created_by,
        created_at,
        is_deleted
    )
    VALUES
    (
        p_role_code,
        p_role_name,
        p_description,
        p_status,
        p_created_by,
        now(),
        false
    )
    RETURNING role_id INTO v_role_id;

    OPEN v_out FOR
    SELECT v_role_id AS role_id;

    RETURN v_out;
END;
$$;


--
-- Name: sys_role_update(bigint, character varying, character varying, character varying, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_role_update(p_role_id bigint, p_role_code character varying, p_role_name character varying, p_description character varying, p_status character varying, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_role_update(
        p_role_id => 1,
        p_role_code => 'ADMIN',
        p_role_name => 'Administrator Updated',
        p_description => 'Updated description',
        p_status => 'active',
        p_updated_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_role_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.sys_role
    SET
        role_code = p_role_code,
        role_name = p_role_name,
        description = p_description,
        status = p_status,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE role_id = p_role_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_role_id AS role_id;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_delete(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_delete(p_user_id bigint, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_delete(1, 1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.sys_user
    SET
        is_deleted = true,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE user_id = p_user_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_user_id AS user_id;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_getbyid(p_user_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        u.user_id,
        u.username,
        u.email,
        u.phone,
        u.password_hash,
        u.full_name,
        u.user_type,
        u.status,
        u.last_login_at,
        u.created_by,
        uc.full_name AS created_by_name,
        u.created_at,
        u.updated_by,
        uu.full_name AS updated_by_name,
        u.updated_at,
        u.is_deleted
    FROM ticketing.sys_user u
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = u.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = u.updated_by
    WHERE u.user_id = p_user_id
      AND u.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_getbyuser(character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_getbyuser(p_username character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_getbyuser(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_getbyuser_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        u.user_id,
        u.username,
        u.email,
        u.phone,
        u.password_hash,
        u.full_name,
        u.user_type,
        u.status,
        u.last_login_at,
        u.created_by,
        uc.full_name AS created_by_name,
        u.created_at,
        u.updated_by,
        uu.full_name AS updated_by_name,
        u.updated_at,
        u.is_deleted
    FROM ticketing.sys_user u
    LEFT JOIN ticketing.sys_user uc
        ON uc.user_id = u.created_by
    LEFT JOIN ticketing.sys_user uu
        ON uu.user_id = u.updated_by
    WHERE u.username = p_username
      AND u.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_getpagedlist(integer, integer, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_user_type => '',
        p_status => ''
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY u.created_at DESC, u.user_id DESC) AS row_index,
            COUNT(*) OVER () AS row_total,
            u.user_id,
            u.username,
            u.email,
            u.phone,
            u.password_hash,
            u.full_name,
            u.user_type,
            u.status,
            u.last_login_at,
            u.created_by,
            uc.full_name AS created_by_name,
            u.created_at,
            u.updated_by,
            uu.full_name AS updated_by_name,
            u.updated_at
        FROM ticketing.sys_user u
        LEFT JOIN ticketing.sys_user uc
            ON uc.user_id = u.created_by
        LEFT JOIN ticketing.sys_user uu
            ON uu.user_id = u.updated_by
        WHERE u.is_deleted = false
          AND (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(u.username) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(u.email, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(u.phone, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(u.full_name) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                trim(coalesce(p_status, '')) = ''
                OR u.status = p_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_insert(character varying, character varying, character varying, character varying, character varying, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_insert(p_username character varying, p_email character varying, p_phone character varying, p_password_hash character varying, p_full_name character varying, p_status character varying, p_created_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_insert(
        p_username => 'admin01',
        p_email => 'admin01@mail.com',
        p_phone => '0909000001',
        p_password_hash => 'hash-value',
        p_full_name => 'Admin User',
        p_user_type => 'admin',
        p_status => 'active',
        p_last_login_at => NULL,
        p_created_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_user_id bigint;
BEGIN
    INSERT INTO ticketing.sys_user
    (
        username,
        email,
        phone,
        password_hash,
        full_name,
        user_type,
        status,
        last_login_at,
        created_by,
        created_at,
        is_deleted
    )
    VALUES
    (
        p_username,
        p_email,
        p_phone,
        p_password_hash,
        p_full_name,
        'admin',
        p_status,
        NULL,
        p_created_by,
        now(),
        false
    )
    RETURNING user_id INTO v_user_id;

    OPEN v_out FOR
    SELECT v_user_id AS user_id;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_role_delete(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_role_delete(p_user_role_id bigint, p_assigned_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_role_delete(1, 1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_role_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.sys_user_role
    SET
        is_deleted = true,
        assigned_by = p_assigned_by
    WHERE user_role_id = p_user_role_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_user_role_id AS user_role_id;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_role_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_role_getbyid(p_user_role_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_role_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_role_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        ur.user_role_id,
        ur.user_id,
        u.username,
        u.full_name AS user_full_name,
        ur.role_id,
        r.role_code,
        r.role_name,
        ur.assigned_at,
        ur.assigned_by,
        ab.full_name AS assigned_by_name,
        ur.is_deleted
    FROM ticketing.sys_user_role ur
    LEFT JOIN ticketing.sys_user u
        ON u.user_id = ur.user_id
    LEFT JOIN ticketing.sys_role r
        ON r.role_id = ur.role_id
    LEFT JOIN ticketing.sys_user ab
        ON ab.user_id = ur.assigned_by
    WHERE ur.user_role_id = p_user_role_id
      AND ur.is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_role_getpagedlist(integer, integer, character varying, bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_role_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_user_id bigint, p_role_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_role_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_user_id => -1,
        p_role_id => -1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_role_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY ur.assigned_at DESC, ur.user_role_id DESC) AS row_index,
            COUNT(*) OVER () AS row_total,
            ur.user_role_id,
            ur.user_id,
            u.username,
            u.full_name AS user_full_name,
            ur.role_id,
            r.role_code,
            r.role_name,
            ur.assigned_at,
            ur.assigned_by,
            ab.full_name AS assigned_by_name
        FROM ticketing.sys_user_role ur
        LEFT JOIN ticketing.sys_user u
            ON u.user_id = ur.user_id
        LEFT JOIN ticketing.sys_role r
            ON r.role_id = ur.role_id
        LEFT JOIN ticketing.sys_user ab
            ON ab.user_id = ur.assigned_by
        WHERE ur.is_deleted = false
          AND (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(coalesce(u.username, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(u.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(r.role_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(r.role_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_user_id = -1
                OR ur.user_id = p_user_id
              )
          AND (
                p_role_id = -1
                OR ur.role_id = p_role_id
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_role_insert(bigint, bigint, timestamp without time zone, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_role_insert(p_user_id bigint, p_role_id bigint, p_assigned_at timestamp without time zone, p_assigned_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_role_insert(
        p_user_id => 1,
        p_role_id => 1,
        p_assigned_at => now(),
        p_assigned_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_role_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_user_role_id bigint;
BEGIN
    INSERT INTO ticketing.sys_user_role
    (
        user_id,
        role_id,
        assigned_at,
        assigned_by,
        is_deleted
    )
    VALUES
    (
        p_user_id,
        p_role_id,
        p_assigned_at,
        p_assigned_by,
        false
    )
    RETURNING user_role_id INTO v_user_role_id;

    OPEN v_out FOR
    SELECT v_user_role_id AS user_role_id;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_role_update(bigint, bigint, bigint, timestamp without time zone, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_role_update(p_user_role_id bigint, p_user_id bigint, p_role_id bigint, p_assigned_at timestamp without time zone, p_assigned_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_role_update(
        p_user_role_id => 1,
        p_user_id => 1,
        p_role_id => 2,
        p_assigned_at => now(),
        p_assigned_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_role_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.sys_user_role
    SET
        user_id = p_user_id,
        role_id = p_role_id,
        assigned_at = p_assigned_at,
        assigned_by = p_assigned_by
    WHERE user_role_id = p_user_role_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_user_role_id AS user_role_id;

    RETURN v_out;
END;
$$;


--
-- Name: sys_user_update(bigint, character varying, character varying, character varying, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.sys_user_update(p_user_id bigint, p_email character varying, p_phone character varying, p_full_name character varying, p_status character varying, p_updated_by bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.sys_user_update(
        p_user_id => 1,
        p_username => 'admin01',
        p_email => 'admin01_new@mail.com',
        p_phone => '0909000002',
        p_password_hash => 'new-hash-value',
        p_full_name => 'Admin User Updated',
        p_user_type => 'admin',
        p_status => 'active',
        p_last_login_at => now(),
        p_updated_by => 1
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'sys_user_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.sys_user
    SET
        email = p_email,
        phone = p_phone,
        full_name = p_full_name,
        user_type = 'admin',
        status = p_status,
        updated_by = p_updated_by,
        updated_at = now()
    WHERE user_id = p_user_id
      AND is_deleted = false;

    OPEN v_out FOR
    SELECT p_user_id AS user_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_delete(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_delete(p_ticket_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.ticket_delete(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    DELETE FROM ticketing.ticket
    WHERE ticket_id = p_ticket_id;

    OPEN v_out FOR
    SELECT p_ticket_id AS ticket_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_getbycustomerid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_getbycustomerid(p_customer_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.ticket_getbycustomerid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_getbycustomerid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        t.ticket_id,
        t.ticket_code,
        t.event_id,
        t.event_name_snapshot,
        t.seat_label_snapshot,
        t.zone_name_snapshot,
        t.ticket_status,
        t.issued_at,
        t.checked_in_at,
        o.order_id,
        o.order_code,
        o.final_amount,
        o.paid_at
        --t.created_at
    FROM ticketing.ticket t
    INNER JOIN ticketing.ticket_order_item oi ON oi.order_item_id = t.order_item_id
    INNER JOIN ticketing.ticket_order o ON o.order_id = oi.order_id
    WHERE t.customer_id = p_customer_id
      --AND t.is_deleted   = FALSE
    ORDER BY t.issued_at DESC;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_getbyid(p_ticket_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'ticket_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        t.ticket_id,
        t.ticket_code,
        t.order_item_id,
        oi.order_id,
        o.order_code,
        t.event_id,
        e.event_code,
        e.event_name,

        t.customer_id,
        c.username  AS customer_username,
        c.full_name AS customer_full_name,
        c.email     AS customer_email,
        c.phone     AS customer_phone,

        t.seat_id,
        vs.seat_code,
        vs.seat_label,
        t.seat_label_snapshot,
        t.zone_name_snapshot,
        t.event_name_snapshot,
        t.ticket_status,
        t.issued_at,
        t.checked_in_at
    FROM ticketing.ticket t
    LEFT JOIN ticketing.ticket_order_item oi
        ON oi.order_item_id = t.order_item_id
    LEFT JOIN ticketing.ticket_order o
        ON o.order_id = oi.order_id
    LEFT JOIN ticketing."event" e
        ON e.event_id = t.event_id
    LEFT JOIN ticketing.customer c
        ON c.customer_id = t.customer_id
    LEFT JOIN ticketing.venue_seat vs
        ON vs.seat_id = t.seat_id
    WHERE t.ticket_id = p_ticket_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_getbyid(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_getbyid(p_ticket_id bigint, p_customer_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.ticket_getbyid(1, 1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        t.ticket_id,
        t.ticket_code,
        t.order_item_id,
        t.event_id,
        t.customer_id,
        t.seat_id,
        t.event_name_snapshot,
        t.seat_label_snapshot,
        t.zone_name_snapshot,
        t.ticket_status,
        t.issued_at,
        t.checked_in_at,
        o.order_id,
        o.order_code,
        o.total_amount,
        o.discount_amount,
        o.final_amount,
        o.paid_at,
        oi.unit_price
        --t.created_at
    FROM ticketing.ticket t
    INNER JOIN ticketing.ticket_order_item oi ON oi.order_item_id = t.order_item_id
    INNER JOIN ticketing.ticket_order o ON o.order_id = oi.order_id
    WHERE t.ticket_id   = p_ticket_id
      AND t.customer_id = p_customer_id;
      --AND t.is_deleted  = FALSE;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_getpagedlist(integer, integer, character varying, bigint, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_event_id bigint, p_customer_id bigint, p_ticket_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'ticket_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY t.issued_at DESC, t.ticket_id DESC) AS row_index,
            COUNT(*) OVER () AS row_total,

            t.ticket_id,
            t.ticket_code,
            t.order_item_id,
            oi.order_id,
            o.order_code,
            t.event_id,
            e.event_code,
            e.event_name,

            t.customer_id,
            c.username  AS customer_username,
            c.full_name AS customer_full_name,
            c.email     AS customer_email,
            c.phone     AS customer_phone,

            t.seat_id,
            vs.seat_code,
            vs.seat_label,
            t.seat_label_snapshot,
            t.zone_name_snapshot,
            t.event_name_snapshot,
            t.ticket_status,
            t.issued_at,
            t.checked_in_at
        FROM ticketing.ticket t
        LEFT JOIN ticketing.ticket_order_item oi
            ON oi.order_item_id = t.order_item_id
        LEFT JOIN ticketing.ticket_order o
            ON o.order_id = oi.order_id
        LEFT JOIN ticketing."event" e
            ON e.event_id = t.event_id
        LEFT JOIN ticketing.customer c
            ON c.customer_id = t.customer_id
        LEFT JOIN ticketing.venue_seat vs
            ON vs.seat_id = t.seat_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(t.ticket_code) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(o.order_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.username, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.email, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.phone, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(vs.seat_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(vs.seat_label, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_event_id = -1
                OR t.event_id = p_event_id
              )
          AND (
                p_customer_id = -1
                OR t.customer_id = p_customer_id
              )
          AND (
                trim(coalesce(p_ticket_status, '')) = ''
                OR t.ticket_status = p_ticket_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_insert(character varying, bigint, bigint, bigint, bigint, character varying, character varying, character varying, character varying, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_insert(p_ticket_code character varying, p_order_item_id bigint, p_event_id bigint, p_customer_id bigint, p_seat_id bigint, p_seat_label_snapshot character varying, p_zone_name_snapshot character varying, p_event_name_snapshot character varying, p_ticket_status character varying, p_issued_at timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'ticket_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_ticket_id bigint;
BEGIN
    INSERT INTO ticketing.ticket
    (
        ticket_code,
        order_item_id,
        event_id,
        customer_id,
        seat_id,
        seat_label_snapshot,
        zone_name_snapshot,
        event_name_snapshot,
        ticket_status,
        issued_at
    )
    VALUES
    (
        p_ticket_code,
        p_order_item_id,
        p_event_id,
        p_customer_id,
        p_seat_id,
        p_seat_label_snapshot,
        p_zone_name_snapshot,
        p_event_name_snapshot,
        p_ticket_status,
        COALESCE(p_issued_at, now())     
    )
    RETURNING ticket_id INTO v_ticket_id;

    OPEN v_out FOR
    SELECT v_ticket_id AS ticket_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_order_delete(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_order_delete(p_order_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.ticket_order_delete(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_order_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    DELETE FROM ticketing.ticket_order
    WHERE order_id = p_order_id;

    OPEN v_out FOR
    SELECT p_order_id AS order_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_order_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_order_getbyid(p_order_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'ticket_order_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        o.order_id,
        o.order_code,
        o.event_id,
        e.event_code,
        e.event_name,

        o.customer_id,
        c.username  AS customer_username,
        c.full_name AS customer_full_name,
        c.email     AS customer_email,
        c.phone     AS customer_phone,

        o.hold_id,
        sh.hold_code,
        o.total_amount,
        o.discount_amount,
        o.final_amount,
        o.order_status,
        o.expired_at,
        o.paid_at,
        o.created_at,
        o.updated_at
    FROM ticketing.ticket_order o
    LEFT JOIN ticketing."event" e
        ON e.event_id = o.event_id
    LEFT JOIN ticketing.customer c
        ON c.customer_id = o.customer_id
    LEFT JOIN ticketing.seat_hold sh
        ON sh.hold_id = o.hold_id
    WHERE o.order_id = p_order_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_order_getpagedlist(integer, integer, character varying, bigint, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_order_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_event_id bigint, p_customer_id bigint, p_order_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'ticket_order_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY o.created_at DESC, o.order_id DESC) AS row_index,
            COUNT(*) OVER () AS row_total,

            o.order_id,
            o.order_code,
            o.event_id,
            e.event_code,
            e.event_name,

            o.customer_id,
            c.username  AS customer_username,
            c.full_name AS customer_full_name,
            c.email     AS customer_email,
            c.phone     AS customer_phone,

            o.hold_id,
            sh.hold_code,
            o.total_amount,
            o.discount_amount,
            o.final_amount,
            o.order_status,
            o.expired_at,
            o.paid_at,
            o.created_at,
            o.updated_at
        FROM ticketing.ticket_order o
        LEFT JOIN ticketing."event" e
            ON e.event_id = o.event_id
        LEFT JOIN ticketing.customer c
            ON c.customer_id = o.customer_id
        LEFT JOIN ticketing.seat_hold sh
            ON sh.hold_id = o.hold_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(o.order_code) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.username, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.full_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.email, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(c.phone, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_event_id = -1
                OR o.event_id = p_event_id
              )
          AND (
                p_customer_id = -1
                OR o.customer_id = p_customer_id
              )
          AND (
                trim(coalesce(p_order_status, '')) = ''
                OR o.order_status = p_order_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_order_insert(character varying, bigint, bigint, bigint, numeric, numeric, numeric, character varying, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_order_insert(p_order_code character varying, p_event_id bigint, p_customer_id bigint, p_hold_id bigint, p_total_amount numeric, p_discount_amount numeric, p_final_amount numeric, p_order_status character varying, p_expired_at timestamp without time zone, p_paid_at timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'ticket_order_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_order_id bigint;
BEGIN
    INSERT INTO ticketing.ticket_order
    (
        order_code,
        event_id,
        customer_id,
        hold_id,
        total_amount,
        discount_amount,
        final_amount,
        order_status,
        expired_at,
        paid_at,
        created_at
    )
    VALUES
    (
        p_order_code,
        p_event_id,
        p_customer_id,
        p_hold_id,
        p_total_amount,
        p_discount_amount,
        p_final_amount,
        p_order_status,
        p_expired_at,
        p_paid_at,
        now()
    )
    RETURNING order_id INTO v_order_id;

    OPEN v_out FOR
    SELECT v_order_id AS order_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_order_item_delete(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_order_item_delete(p_order_item_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.ticket_order_item_delete(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_order_item_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    DELETE FROM ticketing.ticket_order_item
    WHERE order_item_id = p_order_item_id;

    OPEN v_out FOR
    SELECT p_order_item_id AS order_item_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_order_item_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_order_item_getbyid(p_order_item_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.ticket_order_item_getbyid(1);
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_order_item_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT
        oi.order_item_id,
        oi.order_id,
        o.order_code,
        oi.event_seat_inventory_id,
        esi.event_id,
        e.event_code,
        e.event_name,
        oi.seat_id,
        vs.seat_code,
        vs.seat_label,
        oi.zone_id,
        ez.zone_code,
        ez.zone_name,
        oi.unit_price,
        oi.seat_label_snapshot,
        oi.zone_name_snapshot,
        oi.item_status,
        oi.created_at
    FROM ticketing.ticket_order_item oi
    LEFT JOIN ticketing.ticket_order o
        ON o.order_id = oi.order_id
    LEFT JOIN ticketing.event_seat_inventory esi
        ON esi.event_seat_inventory_id = oi.event_seat_inventory_id
    LEFT JOIN ticketing."event" e
        ON e.event_id = esi.event_id
    LEFT JOIN ticketing.venue_seat vs
        ON vs.seat_id = oi.seat_id
    LEFT JOIN ticketing.event_zone ez
        ON ez.event_zone_id = oi.zone_id
    WHERE oi.order_item_id = p_order_item_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_order_item_getpagedlist(integer, integer, character varying, bigint, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_order_item_getpagedlist(p_pagesize integer, p_offset integer, p_keysearch character varying, p_order_id bigint, p_zone_id bigint, p_item_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.ticket_order_item_getpagedlist(
        p_pagesize => 20,
        p_offset => 0,
        p_keysearch => '',
        p_order_id => -1,
        p_zone_id => -1,
        p_item_status => ''
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_order_item_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY oi.created_at DESC, oi.order_item_id DESC) AS row_index,
            COUNT(*) OVER () AS row_total,

            oi.order_item_id,
            oi.order_id,
            o.order_code,
            oi.event_seat_inventory_id,
            esi.event_id,
            e.event_code,
            e.event_name,
            oi.seat_id,
            vs.seat_code,
            vs.seat_label,
            oi.zone_id,
            ez.zone_code,
            ez.zone_name,
            oi.unit_price,
            oi.seat_label_snapshot,
            oi.zone_name_snapshot,
            oi.item_status,
            oi.created_at
        FROM ticketing.ticket_order_item oi
        LEFT JOIN ticketing.ticket_order o
            ON o.order_id = oi.order_id
        LEFT JOIN ticketing.event_seat_inventory esi
            ON esi.event_seat_inventory_id = oi.event_seat_inventory_id
        LEFT JOIN ticketing."event" e
            ON e.event_id = esi.event_id
        LEFT JOIN ticketing.venue_seat vs
            ON vs.seat_id = oi.seat_id
        LEFT JOIN ticketing.event_zone ez
            ON ez.event_zone_id = oi.zone_id
        WHERE (
                trim(coalesce(p_keysearch, '')) = ''
                OR lower(coalesce(o.order_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(e.event_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(vs.seat_code, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(vs.seat_label, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
                OR lower(coalesce(ez.zone_name, '')) LIKE '%' || lower(trim(p_keysearch)) || '%'
              )
          AND (
                p_order_id = -1
                OR oi.order_id = p_order_id
              )
          AND (
                p_zone_id = -1
                OR oi.zone_id = p_zone_id
              )
          AND (
                trim(coalesce(p_item_status, '')) = ''
                OR oi.item_status = p_item_status
              )
    ) t
    ORDER BY t.row_index
    LIMIT p_pagesize OFFSET p_offset;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_order_item_insert(bigint, bigint, bigint, bigint, numeric, character varying, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_order_item_insert(p_order_id bigint, p_event_seat_inventory_id bigint, p_seat_id bigint, p_zone_id bigint, p_unit_price numeric, p_seat_label_snapshot character varying, p_zone_name_snapshot character varying, p_item_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.ticket_order_item_insert(
        p_order_id => 1,
        p_event_seat_inventory_id => 1,
        p_seat_id => 1,
        p_zone_id => 1,
        p_unit_price => 500000,
        p_seat_label_snapshot => 'A01',
        p_zone_name_snapshot => 'VIP',
        p_item_status => 'pending'
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_order_item_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_order_item_id bigint;
BEGIN
    INSERT INTO ticketing.ticket_order_item
    (
        order_id,
        event_seat_inventory_id,
        seat_id,
        zone_id,
        unit_price,
        seat_label_snapshot,
        zone_name_snapshot,
        item_status,
        created_at
    )
    VALUES
    (
        p_order_id,
        p_event_seat_inventory_id,
        p_seat_id,
        p_zone_id,
        p_unit_price,
        p_seat_label_snapshot,
        p_zone_name_snapshot,
        p_item_status,
        now()
    )
    RETURNING order_item_id INTO v_order_item_id;

    OPEN v_out FOR
    SELECT v_order_item_id AS order_item_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_order_item_update(bigint, bigint, bigint, bigint, bigint, numeric, character varying, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_order_item_update(p_order_item_id bigint, p_order_id bigint, p_event_seat_inventory_id bigint, p_seat_id bigint, p_zone_id bigint, p_unit_price numeric, p_seat_label_snapshot character varying, p_zone_name_snapshot character varying, p_item_status character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
/*
    BEGIN;
    SELECT ticketing.ticket_order_item_update(
        p_order_item_id => 1,
        p_order_id => 1,
        p_event_seat_inventory_id => 1,
        p_seat_id => 1,
        p_zone_id => 1,
        p_unit_price => 500000,
        p_seat_label_snapshot => 'A01',
        p_zone_name_snapshot => 'VIP',
        p_item_status => 'paid'
    );
    FETCH ALL IN "<returned_cursor_name>";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_order_item_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.ticket_order_item
    SET
        order_id = p_order_id,
        event_seat_inventory_id = p_event_seat_inventory_id,
        seat_id = p_seat_id,
        zone_id = p_zone_id,
        unit_price = p_unit_price,
        seat_label_snapshot = p_seat_label_snapshot,
        zone_name_snapshot = p_zone_name_snapshot,
        item_status = p_item_status
    WHERE order_item_id = p_order_item_id;

    OPEN v_out FOR
    SELECT p_order_item_id AS order_item_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_order_update(bigint, character varying, bigint, bigint, bigint, numeric, numeric, numeric, character varying, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_order_update(p_order_id bigint, p_order_code character varying, p_event_id bigint, p_customer_id bigint, p_hold_id bigint, p_total_amount numeric, p_discount_amount numeric, p_final_amount numeric, p_order_status character varying, p_expired_at timestamp without time zone, p_paid_at timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'ticket_order_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.ticket_order
    SET
        order_code = p_order_code,
        event_id = p_event_id,
        customer_id = p_customer_id,
        hold_id = p_hold_id,
        total_amount = p_total_amount,
        discount_amount = p_discount_amount,
        final_amount = p_final_amount,
        order_status = p_order_status,
        expired_at = p_expired_at,
        paid_at = p_paid_at,
        updated_at = now()
    WHERE order_id = p_order_id;

    OPEN v_out FOR
    SELECT p_order_id AS order_id;

    RETURN v_out;
END;
$$;


--
-- Name: ticket_update(bigint, character varying, bigint, bigint, bigint, bigint, character varying, character varying, character varying, character varying, timestamp without time zone, timestamp without time zone); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.ticket_update(p_ticket_id bigint, p_ticket_code character varying, p_order_item_id bigint, p_event_id bigint, p_customer_id bigint, p_seat_id bigint, p_seat_label_snapshot character varying, p_zone_name_snapshot character varying, p_event_name_snapshot character varying, p_ticket_status character varying, p_issued_at timestamp without time zone, p_checked_in_at timestamp without time zone) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'ticket_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.ticket
    SET
        ticket_code = p_ticket_code,
        order_item_id = p_order_item_id,
        event_id = p_event_id,
        customer_id = p_customer_id,
        seat_id = p_seat_id,
        seat_label_snapshot = p_seat_label_snapshot,
        zone_name_snapshot = p_zone_name_snapshot,
        event_name_snapshot = p_event_name_snapshot,
        ticket_status = p_ticket_status,
        issued_at = p_issued_at,
        checked_in_at = p_checked_in_at
    WHERE ticket_id = p_ticket_id;

    OPEN v_out FOR
    SELECT p_ticket_id AS ticket_id;

    RETURN v_out;
END;
$$;


--
-- Name: venue_check(bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_check(p_venue_id bigint DEFAULT 0, p_venue_code character varying DEFAULT ''::character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_check_' || replace(gen_random_uuid()::text, '-', '_');
    v_exists integer := 0;
BEGIN
    SELECT CASE
        WHEN EXISTS
        (
            SELECT 1
            FROM ticketing.venue v
            WHERE v.is_deleted = false
              AND v.venue_code = trim(p_venue_code)
              AND (COALESCE(p_venue_id, 0) = 0 OR v.venue_id <> p_venue_id)
        )
        THEN 1
        ELSE 0
    END
    INTO v_exists;

    OPEN ref FOR
    SELECT v_exists AS is_exists;

    RETURN ref;
END;
$$;


--
-- Name: venue_delete(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_delete(p_venue_id bigint, p_deleted_by bigint DEFAULT NULL::bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.venue
    SET
        is_deleted = true,
        updated_by = p_deleted_by,
        updated_at = now()
    WHERE venue_id = p_venue_id
      AND is_deleted = false;

    OPEN ref FOR
    SELECT p_venue_id AS venue_id;

    RETURN ref;
END;
$$;


--
-- Name: venue_getall(); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_getall() RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_getall_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT
        v.venue_id,
        v.venue_code,
        v.venue_name,
        v.address_line,
        v.city,
        v.country,
        v.status,
        v.created_by,
        v.created_at,
        v.updated_by,
        v.updated_at,
        v.is_deleted
    FROM ticketing.venue v
    WHERE  v.is_deleted = false;

    RETURN ref;
END;
$$;


--
-- Name: venue_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_getbyid(p_venue_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT
        v.venue_id,
        v.venue_code,
        v.venue_name,
        v.address_line,
        v.city,
        v.country,
        v.status,
        v.created_by,
        v.created_at,
        v.updated_by,
        v.updated_at,
        v.is_deleted
    FROM ticketing.venue v
    WHERE v.venue_id = p_venue_id
      AND v.is_deleted = false;

    RETURN ref;
END;
$$;


--
-- Name: venue_getpagedlist(integer, integer, character varying, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_getpagedlist(p_pagesize integer DEFAULT 20, p_offset integer DEFAULT 0, p_keysearch character varying DEFAULT ''::character varying, p_status character varying DEFAULT ''::character varying, p_city character varying DEFAULT ''::character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY v.created_at DESC, v.venue_id DESC) AS rowindex,
            COUNT(*) OVER() AS rowtotal,
            v.venue_id,
            v.venue_code,
            v.venue_name,
            v.address_line,
            v.city,
            v.country,
            v.status,
            v.created_by,
            v.created_at,
            v.updated_by,
            v.updated_at
        FROM ticketing.venue v
        WHERE v.is_deleted = false
          AND (
                COALESCE(trim(p_keysearch), '') = ''
                OR v.venue_code ILIKE '%' || trim(p_keysearch) || '%'
                OR v.venue_name ILIKE '%' || trim(p_keysearch) || '%'
                OR COALESCE(v.address_line, '') ILIKE '%' || trim(p_keysearch) || '%'
                OR COALESCE(v.country, '') ILIKE '%' || trim(p_keysearch) || '%'
                OR COALESCE(v.city, '') ILIKE '%' || trim(p_keysearch) || '%'
              )
          AND (
                COALESCE(trim(p_status), '') = ''
                OR v.status = trim(p_status)
              )
          AND (
                COALESCE(trim(p_city), '') = ''
                OR COALESCE(v.city, '') ILIKE '%' || trim(p_city) || '%'
              )
    ) t
    ORDER BY t.rowindex
    LIMIT p_pagesize OFFSET p_offset;

    RETURN ref;
END;
$$;


--
-- Name: venue_insert(character varying, character varying, character varying, character varying, character varying, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_insert(p_venue_code character varying, p_venue_name character varying, p_address_line character varying DEFAULT NULL::character varying, p_city character varying DEFAULT NULL::character varying, p_country character varying DEFAULT NULL::character varying, p_status character varying DEFAULT 'active'::character varying, p_created_by bigint DEFAULT NULL::bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_venue_id bigint;
BEGIN
    INSERT INTO ticketing.venue
    (
        venue_code,
        venue_name,
        address_line,
        city,
        country,
        status,
        created_by,
        created_at,
        is_deleted
    )
    VALUES
    (
        trim(p_venue_code),
        trim(p_venue_name),
        p_address_line,
        p_city,
        p_country,
        p_status,
        p_created_by,
        now(),
        false
    )
    RETURNING venue_id INTO v_venue_id;

    OPEN ref FOR
    SELECT v_venue_id AS venue_id;

    RETURN ref;
END;
$$;


--
-- Name: venue_seat_check(bigint, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_seat_check(p_seat_id bigint DEFAULT 0, p_venue_id bigint DEFAULT 0, p_seat_code character varying DEFAULT ''::character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_seat_check_' || replace(gen_random_uuid()::text, '-', '_');
    v_exists integer := 0;
BEGIN
    SELECT CASE WHEN EXISTS
    (
        SELECT 1
        FROM ticketing.venue_seat s
        WHERE s.is_deleted = false
          AND s.venue_id = p_venue_id
          AND s.seat_code = trim(p_seat_code)
          AND (COALESCE(p_seat_id, 0) = 0 OR s.seat_id <> p_seat_id)
    )
    THEN 1 ELSE 0 END
    INTO v_exists;

    OPEN ref FOR
    SELECT v_exists AS is_exists;

    RETURN ref;
END;
$$;


--
-- Name: venue_seat_delete(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_seat_delete(p_seat_id bigint, p_deleted_by bigint DEFAULT NULL::bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_seat_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.venue_seat
    SET
        is_deleted = true,
        updated_by = p_deleted_by,
        updated_at = now()
    WHERE seat_id = p_seat_id
      AND is_deleted = false;

    OPEN ref FOR
    SELECT p_seat_id AS seat_id;

    RETURN ref;
END;
$$;


--
-- Name: venue_seat_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_seat_getbyid(p_seat_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_seat_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT
        s.seat_id,
        s.venue_id,
        v.venue_code,
        v.venue_name,
        s.section_id,
        sec.section_code,
        sec.section_name,
        s.seat_code,
        s.row_label,
        s.seat_number,
        s.seat_label,
        s.x_pos,
        s.y_pos,
        s.seat_type,
        s.status,
        s.created_by,
        s.created_at,
        s.updated_by,
        s.updated_at,
        s.is_deleted
    FROM ticketing.venue_seat s
    INNER JOIN ticketing.venue v ON v.venue_id = s.venue_id
    INNER JOIN ticketing.venue_section sec ON sec.section_id = s.section_id
    WHERE s.seat_id = p_seat_id
      AND s.is_deleted = false
      AND v.is_deleted = false
      AND sec.is_deleted = false;

    RETURN ref;
END;
$$;


--
-- Name: venue_seat_getbysectionids(bigint[]); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_seat_getbysectionids(p_section_ids bigint[]) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_out refcursor := 'venue_seat_getbysectionids_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN v_out FOR
    SELECT *
    FROM ticketing.venue_seat
    WHERE section_id = ANY(p_section_ids)
      AND is_deleted = false;

    RETURN v_out;
END;
$$;


--
-- Name: venue_seat_getbyvenueid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_seat_getbyvenueid(p_venue_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_seat_getbyvenueid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT
        s.seat_id,
        s.venue_id,
        v.venue_code,
        v.venue_name,
        s.section_id,
        sec.section_code,
        sec.section_name,
        s.seat_code,
        s.row_label,
        s.seat_number,
        s.seat_label,
        s.x_pos,
        s.y_pos,
        s.seat_type,
        s.status,
        s.created_by,
        s.created_at,
        s.updated_by,
        s.updated_at,
        s.is_deleted
    FROM ticketing.venue_seat s
    INNER JOIN ticketing.venue v ON v.venue_id = s.venue_id
    INNER JOIN ticketing.venue_section sec ON sec.section_id = s.section_id
    WHERE s.venue_id = p_venue_id
      AND s.is_deleted = false
      AND v.is_deleted = false
      AND sec.is_deleted = false;

    RETURN ref;
END;
$$;


--
-- Name: venue_seat_getpagedlist(integer, integer, bigint, bigint, character varying, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_seat_getpagedlist(p_pagesize integer DEFAULT 20, p_offset integer DEFAULT 0, p_venue_id bigint DEFAULT 0, p_section_id bigint DEFAULT 0, p_keysearch character varying DEFAULT ''::character varying, p_status character varying DEFAULT ''::character varying, p_seat_type character varying DEFAULT ''::character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_seat_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY s.created_at DESC, s.seat_id DESC) AS rowindex,
            COUNT(*) OVER() AS rowtotal,
            s.seat_id,
            s.venue_id,
            v.venue_code,
            v.venue_name,
            s.section_id,
            sec.section_code,
            sec.section_name,
            s.seat_code,
            s.row_label,
            s.seat_number,
            s.seat_label,
            s.x_pos,
            s.y_pos,
            s.seat_type,
            s.status,
            s.created_by,
            s.created_at,
            s.updated_by,
            s.updated_at
        FROM ticketing.venue_seat s
        INNER JOIN ticketing.venue v ON v.venue_id = s.venue_id
        INNER JOIN ticketing.venue_section sec ON sec.section_id = s.section_id
        WHERE s.is_deleted = false
          AND v.is_deleted = false
          AND sec.is_deleted = false
          AND (
                COALESCE(p_venue_id, 0) = 0
                OR s.venue_id = p_venue_id
              )
          AND (
                COALESCE(p_section_id, 0) = 0
                OR s.section_id = p_section_id
              )
          AND (
                COALESCE(trim(p_keysearch), '') = ''
                OR s.seat_code ILIKE '%' || trim(p_keysearch) || '%'
                OR COALESCE(s.row_label, '') ILIKE '%' || trim(p_keysearch) || '%'
                OR COALESCE(s.seat_number, '') ILIKE '%' || trim(p_keysearch) || '%'
                OR COALESCE(s.seat_label, '') ILIKE '%' || trim(p_keysearch) || '%'
                OR sec.section_code ILIKE '%' || trim(p_keysearch) || '%'
                OR sec.section_name ILIKE '%' || trim(p_keysearch) || '%'
              )
          AND (
                COALESCE(trim(p_status), '') = ''
                OR s.status = trim(p_status)
              )
          AND (
                COALESCE(trim(p_seat_type), '') = ''
                OR s.seat_type = trim(p_seat_type)
              )
    ) t
    ORDER BY t.rowindex
    LIMIT p_pagesize OFFSET p_offset;

    RETURN ref;
END;
$$;


--
-- Name: venue_seat_getsectionid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_seat_getsectionid(p_section_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_seat_getsectionid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT
        s.seat_id,
        s.venue_id,
        v.venue_code,
        v.venue_name,
        s.section_id,
        sec.section_code,
        sec.section_name,
        s.seat_code,
        s.row_label,
        s.seat_number,
        s.seat_label,
        s.x_pos,
        s.y_pos,
        s.seat_type,
        s.status,
        s.created_by,
        s.created_at,
        s.updated_by,
        s.updated_at,
        s.is_deleted
    FROM ticketing.venue_seat s
    INNER JOIN ticketing.venue v ON v.venue_id = s.venue_id
    INNER JOIN ticketing.venue_section sec ON sec.section_id = s.section_id
    WHERE s.section_id = p_section_id
      AND s.is_deleted = false
      AND v.is_deleted = false
      AND sec.is_deleted = false;

    RETURN ref;
END;
$$;


--
-- Name: venue_seat_insert(bigint, bigint, character varying, character varying, character varying, character varying, numeric, numeric, character varying, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_seat_insert(p_venue_id bigint, p_section_id bigint, p_seat_code character varying, p_row_label character varying DEFAULT NULL::character varying, p_seat_number character varying DEFAULT NULL::character varying, p_seat_label character varying DEFAULT NULL::character varying, p_x_pos numeric DEFAULT NULL::numeric, p_y_pos numeric DEFAULT NULL::numeric, p_seat_type character varying DEFAULT 'seat'::character varying, p_status character varying DEFAULT 'active'::character varying, p_created_by bigint DEFAULT NULL::bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_seat_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_seat_id bigint;
BEGIN
    INSERT INTO ticketing.venue_seat
    (
        venue_id,
        section_id,
        seat_code,
        row_label,
        seat_number,
        seat_label,
        x_pos,
        y_pos,
        seat_type,
        status,
        created_by,
        created_at,
        is_deleted
    )
    VALUES
    (
        p_venue_id,
        p_section_id,
        trim(p_seat_code),
        p_row_label,
        p_seat_number,
        p_seat_label,
        p_x_pos,
        p_y_pos,
        p_seat_type,
        p_status,
        p_created_by,
        now(),
        false
    )
    RETURNING seat_id INTO v_seat_id;

    OPEN ref FOR
    SELECT v_seat_id AS seat_id;

    RETURN ref;
END;
$$;


--
-- Name: venue_seat_update(bigint, bigint, bigint, character varying, character varying, character varying, character varying, numeric, numeric, character varying, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_seat_update(p_seat_id bigint, p_venue_id bigint, p_section_id bigint, p_seat_code character varying, p_row_label character varying DEFAULT NULL::character varying, p_seat_number character varying DEFAULT NULL::character varying, p_seat_label character varying DEFAULT NULL::character varying, p_x_pos numeric DEFAULT NULL::numeric, p_y_pos numeric DEFAULT NULL::numeric, p_seat_type character varying DEFAULT 'seat'::character varying, p_status character varying DEFAULT 'active'::character varying, p_updated_by bigint DEFAULT NULL::bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_seat_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.venue_seat
    SET
        venue_id     = p_venue_id,
        section_id   = p_section_id,
        seat_code    = trim(p_seat_code),
        row_label    = p_row_label,
        seat_number  = p_seat_number,
        seat_label   = p_seat_label,
        x_pos        = p_x_pos,
        y_pos        = p_y_pos,
        seat_type    = p_seat_type,
        status       = p_status,
        updated_by   = p_updated_by,
        updated_at   = now()
    WHERE seat_id = p_seat_id
      AND is_deleted = false;

    OPEN ref FOR
    SELECT p_seat_id AS seat_id;

    RETURN ref;
END;
$$;


--
-- Name: venue_section_check(bigint, bigint, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_section_check(p_section_id bigint DEFAULT 0, p_venue_id bigint DEFAULT 0, p_section_code character varying DEFAULT ''::character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_section_check_' || replace(gen_random_uuid()::text, '-', '_');
    v_exists integer := 0;
BEGIN
    SELECT CASE WHEN EXISTS
    (
        SELECT 1
        FROM ticketing.venue_section s
        WHERE s.is_deleted = false
          AND s.venue_id = p_venue_id
          AND s.section_code = trim(p_section_code)
          AND (COALESCE(p_section_id, 0) = 0 OR s.section_id <> p_section_id)
    )
    THEN 1 ELSE 0 END
    INTO v_exists;

    OPEN ref FOR
    SELECT v_exists AS is_exists;

    RETURN ref;
END;
$$;


--
-- Name: venue_section_delete(bigint, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_section_delete(p_section_id bigint, p_deleted_by bigint DEFAULT NULL::bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_section_delete_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.venue_section
    SET
        is_deleted = true,
        updated_by = p_deleted_by,
        updated_at = now()
    WHERE section_id = p_section_id
      AND is_deleted = false;

    OPEN ref FOR
    SELECT p_section_id AS section_id;

    RETURN ref;
END;
$$;


--
-- Name: venue_section_getall(); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_section_getall() RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_section_getall_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT
        s.section_id,
        s.venue_id,
        s.section_code,
        s.section_name,
        s.display_order,
        s.status,
        s.created_by,
        s.created_at,
        s.updated_by,
        s.updated_at,
        s.is_deleted
    FROM ticketing.venue_section s
    WHERE s.is_deleted = false;

    RETURN ref;
END;
$$;


--
-- Name: venue_section_getbyid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_section_getbyid(p_section_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_section_getbyid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT
        s.section_id,
        s.venue_id,
        s.section_code,
        s.section_name,
        s.display_order,
        s.status,
        s.created_by,
        s.created_at,
        s.updated_by,
        s.updated_at,
        s.is_deleted
    FROM ticketing.venue_section s
    WHERE s.section_id = p_section_id
      AND s.is_deleted = false;

    RETURN ref;
END;
$$;


--
-- Name: venue_section_getbyvenueid(bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_section_getbyvenueid(p_venue_id bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_section_getbyvenueid_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT
        s.section_id,
        s.venue_id,
        s.section_code,
        s.section_name,
        s.display_order,
        s.status,
        s.created_by,
        s.created_at,
        s.updated_by,
        s.updated_at,
        s.is_deleted
    FROM ticketing.venue_section s
    WHERE s.venue_id = p_venue_id
      AND s.is_deleted = false;

    RETURN ref;
END;
$$;


--
-- Name: venue_section_getpagedlist(integer, integer, bigint, character varying, character varying); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_section_getpagedlist(p_pagesize integer DEFAULT 20, p_offset integer DEFAULT 0, p_venue_id bigint DEFAULT 0, p_keysearch character varying DEFAULT ''::character varying, p_status character varying DEFAULT ''::character varying) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_section_getpagedlist_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    OPEN ref FOR
    SELECT *
    FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY s.display_order ASC, s.section_id DESC) AS rowindex,
            COUNT(*) OVER() AS rowtotal,
            s.section_id,
            s.venue_id,
            v.venue_code,
            v.venue_name,
            s.section_code,
            s.section_name,
            s.display_order,
            s.status,
            s.created_by,
            s.created_at,
            s.updated_by,
            s.updated_at
        FROM ticketing.venue_section s
        INNER JOIN ticketing.venue v ON v.venue_id = s.venue_id
        WHERE s.is_deleted = false
          AND v.is_deleted = false
          AND (
                COALESCE(p_venue_id, 0) = 0
                OR s.venue_id = p_venue_id
              )
          AND (
                COALESCE(trim(p_keysearch), '') = ''
                OR s.section_code ILIKE '%' || trim(p_keysearch) || '%'
                OR s.section_name ILIKE '%' || trim(p_keysearch) || '%'
                OR v.venue_code ILIKE '%' || trim(p_keysearch) || '%'
                OR v.venue_name ILIKE '%' || trim(p_keysearch) || '%'
              )
          AND (
                COALESCE(trim(p_status), '') = ''
                OR s.status = trim(p_status)
              )
    ) t
    ORDER BY t.rowindex
    LIMIT p_pagesize OFFSET p_offset;

    RETURN ref;
END;
$$;


--
-- Name: venue_section_insert(bigint, character varying, character varying, integer, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_section_insert(p_venue_id bigint, p_section_code character varying, p_section_name character varying, p_display_order integer DEFAULT 0, p_status character varying DEFAULT 'active'::character varying, p_created_by bigint DEFAULT NULL::bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_section_insert_' || replace(gen_random_uuid()::text, '-', '_');
    v_section_id bigint;
BEGIN
    INSERT INTO ticketing.venue_section
    (
        venue_id,
        section_code,
        section_name,
        display_order,
        status,
        created_by,
        created_at,
        is_deleted
    )
    VALUES
    (
        p_venue_id,
        trim(p_section_code),
        trim(p_section_name),
        p_display_order,
        p_status,
        p_created_by,
        now(),
        false
    )
    RETURNING section_id INTO v_section_id;

    OPEN ref FOR
    SELECT v_section_id AS section_id;

    RETURN ref;
END;
$$;


--
-- Name: venue_section_update(bigint, bigint, character varying, character varying, integer, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_section_update(p_section_id bigint, p_venue_id bigint, p_section_code character varying, p_section_name character varying, p_display_order integer DEFAULT 0, p_status character varying DEFAULT 'active'::character varying, p_updated_by bigint DEFAULT NULL::bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_section_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.venue_section
    SET
        venue_id       = p_venue_id,
        section_code   = trim(p_section_code),
        section_name   = trim(p_section_name),
        display_order  = p_display_order,
        status         = p_status,
        updated_by     = p_updated_by,
        updated_at     = now()
    WHERE section_id = p_section_id
      AND is_deleted = false;

    OPEN ref FOR
    SELECT p_section_id AS section_id;

    RETURN ref;
END;
$$;


--
-- Name: venue_update(bigint, character varying, character varying, character varying, character varying, character varying, character varying, bigint); Type: FUNCTION; Schema: ticketing; Owner: -
--

CREATE FUNCTION ticketing.venue_update(p_venue_id bigint, p_venue_code character varying, p_venue_name character varying, p_address_line character varying DEFAULT NULL::character varying, p_city character varying DEFAULT NULL::character varying, p_country character varying DEFAULT NULL::character varying, p_status character varying DEFAULT 'active'::character varying, p_updated_by bigint DEFAULT NULL::bigint) RETURNS refcursor
    LANGUAGE plpgsql
    AS $$
DECLARE
    ref refcursor := 'venue_update_' || replace(gen_random_uuid()::text, '-', '_');
BEGIN
    UPDATE ticketing.venue
    SET
        venue_code   = trim(p_venue_code),
        venue_name   = trim(p_venue_name),
        address_line = p_address_line,
        city         = p_city,
        country      = p_country,
        status       = p_status,
        updated_by   = p_updated_by,
        updated_at   = now()
    WHERE venue_id = p_venue_id
      AND is_deleted = false;

    OPEN ref FOR
    SELECT p_venue_id AS venue_id;

    RETURN ref;
END;
$$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: audit_log; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.audit_log (
    audit_log_id bigint NOT NULL,
    actor_user_id bigint,
    module_name character varying(50) NOT NULL,
    action_name character varying(50) NOT NULL,
    entity_name character varying(50) NOT NULL,
    entity_id bigint,
    old_data text,
    new_data text,
    ip_address character varying(50),
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL
);


--
-- Name: TABLE audit_log; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.audit_log IS 'Lưu log thao tác quản trị và thay đổi nghiệp vụ quan trọng trong hệ thống.';


--
-- Name: COLUMN audit_log.audit_log_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.audit_log.audit_log_id IS 'Khóa chính định danh log audit.';


--
-- Name: COLUMN audit_log.actor_user_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.audit_log.actor_user_id IS 'Người thực hiện thao tác.';


--
-- Name: COLUMN audit_log.module_name; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.audit_log.module_name IS 'Tên module phát sinh thao tác, ví dụ event, venue, order, hold.';


--
-- Name: COLUMN audit_log.action_name; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.audit_log.action_name IS 'Tên hành động, ví dụ create, update, publish, cancel, release_hold.';


--
-- Name: COLUMN audit_log.entity_name; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.audit_log.entity_name IS 'Tên entity bị tác động.';


--
-- Name: COLUMN audit_log.entity_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.audit_log.entity_id IS 'Id của entity bị tác động.';


--
-- Name: COLUMN audit_log.old_data; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.audit_log.old_data IS 'Dữ liệu cũ trước khi thay đổi, thường lưu dạng JSON text.';


--
-- Name: COLUMN audit_log.new_data; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.audit_log.new_data IS 'Dữ liệu mới sau khi thay đổi, thường lưu dạng JSON text.';


--
-- Name: COLUMN audit_log.ip_address; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.audit_log.ip_address IS 'Địa chỉ IP của nguồn phát sinh thao tác.';


--
-- Name: COLUMN audit_log.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.audit_log.created_at IS 'Thời điểm phát sinh log audit.';


--
-- Name: audit_log_audit_log_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.audit_log_audit_log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: audit_log_audit_log_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.audit_log_audit_log_id_seq OWNED BY ticketing.audit_log.audit_log_id;


--
-- Name: customer; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.customer (
    customer_id bigint NOT NULL,
    customer_code character varying(50) NOT NULL,
    username character varying(50) NOT NULL,
    email character varying(100),
    phone character varying(20),
    password_hash character varying(255) NOT NULL,
    full_name character varying(100) NOT NULL,
    avatar_url character varying(255),
    status character varying(20) DEFAULT 'active'::character varying NOT NULL,
    email_verified boolean DEFAULT false,
    last_login_at timestamp without time zone,
    created_at timestamp without time zone DEFAULT now(),
    updated_at timestamp without time zone,
    is_deleted boolean DEFAULT false
);


--
-- Name: customer_customer_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.customer_customer_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: customer_customer_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.customer_customer_id_seq OWNED BY ticketing.customer.customer_id;


--
-- Name: event; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.event (
    event_id bigint NOT NULL,
    event_code character varying(50) NOT NULL,
    event_name character varying(255) NOT NULL,
    description text,
    venue_id bigint NOT NULL,
    banner_url character varying(1000),
    start_time timestamp(3) without time zone NOT NULL,
    end_time timestamp(3) without time zone NOT NULL,
    sale_start_time timestamp(3) without time zone,
    sale_end_time timestamp(3) without time zone,
    status character varying(30) NOT NULL,
    published_at timestamp(3) without time zone,
    on_sale_at timestamp(3) without time zone,
    created_by bigint,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_by bigint,
    updated_at timestamp(3) without time zone,
    is_deleted boolean DEFAULT false NOT NULL,
    is_featured boolean DEFAULT false NOT NULL,
    is_trending boolean DEFAULT false NOT NULL,
    display_order integer DEFAULT 0 NOT NULL
);


--
-- Name: TABLE event; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.event IS 'Thông tin sự kiện được mở bán vé trong hệ thống.';


--
-- Name: COLUMN event.event_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.event_id IS 'Khóa chính định danh sự kiện.';


--
-- Name: COLUMN event.event_code; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.event_code IS 'Mã sự kiện duy nhất.';


--
-- Name: COLUMN event.event_name; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.event_name IS 'Tên sự kiện.';


--
-- Name: COLUMN event.description; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.description IS 'Mô tả chi tiết sự kiện.';


--
-- Name: COLUMN event.venue_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.venue_id IS 'Tham chiếu địa điểm tổ chức sự kiện.';


--
-- Name: COLUMN event.banner_url; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.banner_url IS 'Đường dẫn ảnh banner sự kiện.';


--
-- Name: COLUMN event.start_time; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.start_time IS 'Thời gian bắt đầu diễn ra sự kiện.';


--
-- Name: COLUMN event.end_time; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.end_time IS 'Thời gian kết thúc sự kiện.';


--
-- Name: COLUMN event.sale_start_time; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.sale_start_time IS 'Thời điểm bắt đầu mở bán vé.';


--
-- Name: COLUMN event.sale_end_time; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.sale_end_time IS 'Thời điểm kết thúc bán vé.';


--
-- Name: COLUMN event.status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.status IS 'Trạng thái sự kiện: draft, published, on_sale, sale_closed, ended, cancelled.';


--
-- Name: COLUMN event.published_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.published_at IS 'Thời điểm sự kiện được publish.';


--
-- Name: COLUMN event.on_sale_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.on_sale_at IS 'Thời điểm sự kiện được chuyển sang trạng thái mở bán.';


--
-- Name: COLUMN event.created_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.created_by IS 'Người tạo bản ghi.';


--
-- Name: COLUMN event.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.created_at IS 'Thời điểm tạo bản ghi.';


--
-- Name: COLUMN event.updated_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.updated_by IS 'Người cập nhật bản ghi gần nhất.';


--
-- Name: COLUMN event.updated_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.updated_at IS 'Thời điểm cập nhật gần nhất.';


--
-- Name: COLUMN event.is_deleted; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event.is_deleted IS 'Đánh dấu xóa mềm bản ghi.';


--
-- Name: event_event_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.event_event_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: event_event_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.event_event_id_seq OWNED BY ticketing.event.event_id;


--
-- Name: event_publish_log; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.event_publish_log (
    event_publish_log_id bigint NOT NULL,
    event_id bigint NOT NULL,
    action character varying(30) NOT NULL,
    old_status character varying(30),
    new_status character varying(30) NOT NULL,
    changed_by bigint,
    changed_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    note character varying(1000),
    CONSTRAINT ck_event_publish_log_action CHECK (((action)::text = ANY ((ARRAY['publish'::character varying, 'open_sale'::character varying, 'close_sale'::character varying, 'cancel'::character varying])::text[]))),
    CONSTRAINT ck_event_publish_log_new_status CHECK (((new_status)::text = ANY ((ARRAY['draft'::character varying, 'published'::character varying, 'on_sale'::character varying, 'sale_closed'::character varying, 'ended'::character varying, 'cancelled'::character varying])::text[]))),
    CONSTRAINT ck_event_publish_log_old_status CHECK (((old_status IS NULL) OR ((old_status)::text = ANY ((ARRAY['draft'::character varying, 'published'::character varying, 'on_sale'::character varying, 'sale_closed'::character varying, 'ended'::character varying, 'cancelled'::character varying])::text[]))))
);


--
-- Name: TABLE event_publish_log; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.event_publish_log IS 'Lưu lịch sử thay đổi trạng thái publish và mở bán của sự kiện.';


--
-- Name: COLUMN event_publish_log.event_publish_log_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_publish_log.event_publish_log_id IS 'Khóa chính định danh log publish sự kiện.';


--
-- Name: COLUMN event_publish_log.event_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_publish_log.event_id IS 'Tham chiếu sự kiện.';


--
-- Name: COLUMN event_publish_log.action; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_publish_log.action IS 'Loại thao tác trạng thái: publish, open_sale, close_sale, cancel.';


--
-- Name: COLUMN event_publish_log.old_status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_publish_log.old_status IS 'Trạng thái cũ của sự kiện trước khi thay đổi.';


--
-- Name: COLUMN event_publish_log.new_status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_publish_log.new_status IS 'Trạng thái mới của sự kiện sau khi thay đổi.';


--
-- Name: COLUMN event_publish_log.changed_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_publish_log.changed_by IS 'Người thực hiện thay đổi trạng thái.';


--
-- Name: COLUMN event_publish_log.changed_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_publish_log.changed_at IS 'Thời điểm thay đổi trạng thái.';


--
-- Name: COLUMN event_publish_log.note; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_publish_log.note IS 'Ghi chú bổ sung cho lần thay đổi trạng thái.';


--
-- Name: event_publish_log_event_publish_log_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.event_publish_log_event_publish_log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: event_publish_log_event_publish_log_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.event_publish_log_event_publish_log_id_seq OWNED BY ticketing.event_publish_log.event_publish_log_id;


--
-- Name: event_seat_inventory; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.event_seat_inventory (
    event_seat_inventory_id bigint NOT NULL,
    event_id bigint NOT NULL,
    seat_id bigint NOT NULL,
    event_zone_id bigint NOT NULL,
    seat_status character varying(20) NOT NULL,
    current_hold_id bigint,
    current_order_item_id bigint,
    base_price numeric(18,2),
    version integer DEFAULT 1 NOT NULL,
    updated_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    CONSTRAINT ck_event_seat_inventory_base_price CHECK (((base_price IS NULL) OR (base_price >= (0)::numeric))),
    CONSTRAINT ck_event_seat_inventory_status CHECK (((seat_status)::text = ANY ((ARRAY['available'::character varying, 'held'::character varying, 'sold'::character varying, 'locked'::character varying, 'unavailable'::character varying])::text[]))),
    CONSTRAINT ck_event_seat_inventory_version CHECK ((version >= 1))
);


--
-- Name: TABLE event_seat_inventory; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.event_seat_inventory IS 'Bảng inventory runtime của ghế theo từng event, là nguồn trạng thái bán vé thực tế.';


--
-- Name: COLUMN event_seat_inventory.event_seat_inventory_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_seat_inventory.event_seat_inventory_id IS 'Khóa chính định danh inventory ghế theo event.';


--
-- Name: COLUMN event_seat_inventory.event_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_seat_inventory.event_id IS 'Tham chiếu sự kiện.';


--
-- Name: COLUMN event_seat_inventory.seat_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_seat_inventory.seat_id IS 'Tham chiếu ghế master của venue.';


--
-- Name: COLUMN event_seat_inventory.event_zone_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_seat_inventory.event_zone_id IS 'Tham chiếu zone của sự kiện mà ghế thuộc về.';


--
-- Name: COLUMN event_seat_inventory.seat_status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_seat_inventory.seat_status IS 'Trạng thái runtime của ghế: available, held, sold, locked, unavailable.';


--
-- Name: COLUMN event_seat_inventory.current_hold_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_seat_inventory.current_hold_id IS 'Hold hiện tại đang giữ ghế, nếu có.';


--
-- Name: COLUMN event_seat_inventory.current_order_item_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_seat_inventory.current_order_item_id IS 'Order item hiện tại đã chốt mua ghế, nếu có.';


--
-- Name: COLUMN event_seat_inventory.base_price; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_seat_inventory.base_price IS 'Giá cơ bản snapshot của ghế tại thời điểm generate inventory.';


--
-- Name: COLUMN event_seat_inventory.version; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_seat_inventory.version IS 'Phiên bản bản ghi dùng hỗ trợ optimistic update nếu cần.';


--
-- Name: COLUMN event_seat_inventory.updated_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_seat_inventory.updated_at IS 'Thời điểm cập nhật trạng thái ghế gần nhất.';


--
-- Name: event_seat_inventory_event_seat_inventory_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.event_seat_inventory_event_seat_inventory_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: event_seat_inventory_event_seat_inventory_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.event_seat_inventory_event_seat_inventory_id_seq OWNED BY ticketing.event_seat_inventory.event_seat_inventory_id;


--
-- Name: event_zone; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.event_zone (
    event_zone_id bigint NOT NULL,
    event_id bigint NOT NULL,
    zone_code character varying(50) NOT NULL,
    zone_name character varying(100) NOT NULL,
    color_hex character varying(20),
    description character varying(500),
    display_order integer DEFAULT 0 NOT NULL,
    status character varying(20) NOT NULL,
    created_by bigint,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_by bigint,
    updated_at timestamp(3) without time zone,
    is_deleted boolean DEFAULT false NOT NULL
);


--
-- Name: TABLE event_zone; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.event_zone IS 'Danh mục zone bán vé của một sự kiện, dùng để nhóm ghế và cấu hình giá.';


--
-- Name: COLUMN event_zone.event_zone_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.event_zone_id IS 'Khóa chính định danh zone của sự kiện.';


--
-- Name: COLUMN event_zone.event_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.event_id IS 'Tham chiếu sự kiện.';


--
-- Name: COLUMN event_zone.zone_code; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.zone_code IS 'Mã zone duy nhất trong phạm vi event.';


--
-- Name: COLUMN event_zone.zone_name; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.zone_name IS 'Tên hiển thị zone.';


--
-- Name: COLUMN event_zone.color_hex; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.color_hex IS 'Mã màu dùng để hiển thị zone trên seat map.';


--
-- Name: COLUMN event_zone.description; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.description IS 'Mô tả thêm cho zone.';


--
-- Name: COLUMN event_zone.display_order; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.display_order IS 'Thứ tự hiển thị của zone.';


--
-- Name: COLUMN event_zone.status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.status IS 'Trạng thái zone: active hoặc inactive.';


--
-- Name: COLUMN event_zone.created_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.created_by IS 'Người tạo bản ghi.';


--
-- Name: COLUMN event_zone.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.created_at IS 'Thời điểm tạo bản ghi.';


--
-- Name: COLUMN event_zone.updated_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.updated_by IS 'Người cập nhật bản ghi gần nhất.';


--
-- Name: COLUMN event_zone.updated_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.updated_at IS 'Thời điểm cập nhật gần nhất.';


--
-- Name: COLUMN event_zone.is_deleted; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone.is_deleted IS 'Đánh dấu xóa mềm bản ghi.';


--
-- Name: event_zone_event_zone_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.event_zone_event_zone_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: event_zone_event_zone_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.event_zone_event_zone_id_seq OWNED BY ticketing.event_zone.event_zone_id;


--
-- Name: event_zone_price; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.event_zone_price (
    event_zone_price_id bigint NOT NULL,
    event_zone_id bigint NOT NULL,
    price numeric(18,2) NOT NULL,
    currency character varying(10) DEFAULT 'VND'::character varying NOT NULL,
    start_time timestamp(3) without time zone,
    end_time timestamp(3) without time zone,
    is_active boolean DEFAULT true NOT NULL,
    created_by bigint,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_by bigint,
    updated_at timestamp(3) without time zone,
    is_deleted boolean DEFAULT false NOT NULL
);


--
-- Name: TABLE event_zone_price; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.event_zone_price IS 'Cấu hình giá bán của zone theo event, có thể hỗ trợ hiệu lực theo thời gian.';


--
-- Name: COLUMN event_zone_price.event_zone_price_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.event_zone_price_id IS 'Khóa chính định danh cấu hình giá zone.';


--
-- Name: COLUMN event_zone_price.event_zone_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.event_zone_id IS 'Tham chiếu zone của sự kiện.';


--
-- Name: COLUMN event_zone_price.price; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.price IS 'Giá bán áp dụng cho zone.';


--
-- Name: COLUMN event_zone_price.currency; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.currency IS 'Đơn vị tiền tệ, mặc định VND.';


--
-- Name: COLUMN event_zone_price.start_time; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.start_time IS 'Thời điểm bắt đầu hiệu lực của giá.';


--
-- Name: COLUMN event_zone_price.end_time; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.end_time IS 'Thời điểm kết thúc hiệu lực của giá.';


--
-- Name: COLUMN event_zone_price.is_active; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.is_active IS 'Đánh dấu cấu hình giá đang hoạt động.';


--
-- Name: COLUMN event_zone_price.created_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.created_by IS 'Người tạo bản ghi.';


--
-- Name: COLUMN event_zone_price.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.created_at IS 'Thời điểm tạo bản ghi.';


--
-- Name: COLUMN event_zone_price.updated_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.updated_by IS 'Người cập nhật bản ghi gần nhất.';


--
-- Name: COLUMN event_zone_price.updated_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.updated_at IS 'Thời điểm cập nhật gần nhất.';


--
-- Name: COLUMN event_zone_price.is_deleted; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.event_zone_price.is_deleted IS 'Đánh dấu xóa mềm bản ghi.';


--
-- Name: event_zone_price_event_zone_price_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.event_zone_price_event_zone_price_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: event_zone_price_event_zone_price_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.event_zone_price_event_zone_price_id_seq OWNED BY ticketing.event_zone_price.event_zone_price_id;


--
-- Name: event_zone_section; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.event_zone_section (
    event_zone_section_id bigint NOT NULL,
    event_id bigint NOT NULL,
    event_zone_id bigint NOT NULL,
    section_id bigint NOT NULL,
    created_by bigint,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_by bigint,
    updated_at timestamp(3) without time zone,
    is_deleted boolean DEFAULT false NOT NULL
);


--
-- Name: event_zone_section_event_zone_section_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.event_zone_section_event_zone_section_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: event_zone_section_event_zone_section_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.event_zone_section_event_zone_section_id_seq OWNED BY ticketing.event_zone_section.event_zone_section_id;


--
-- Name: idempotency_request; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.idempotency_request (
    idempotency_id bigint NOT NULL,
    idempotency_key character varying(100) NOT NULL,
    request_type character varying(50) NOT NULL,
    customer_id bigint,
    request_hash character varying(200),
    status character varying(20) NOT NULL,
    response_snapshot text,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    expired_at timestamp(3) without time zone,
    CONSTRAINT ck_idempotency_request_status CHECK (((status)::text = ANY ((ARRAY['processing'::character varying, 'completed'::character varying, 'failed'::character varying, 'expired'::character varying])::text[]))),
    CONSTRAINT ck_idempotency_request_type CHECK (((request_type)::text = ANY ((ARRAY['hold_seats'::character varying, 'checkout'::character varying, 'payment_callback'::character varying])::text[])))
);


--
-- Name: TABLE idempotency_request; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.idempotency_request IS 'Lưu request idempotency để chống double click và retry lặp cho các API quan trọng.';


--
-- Name: COLUMN idempotency_request.idempotency_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.idempotency_request.idempotency_id IS 'Khóa chính định danh bản ghi idempotency.';


--
-- Name: COLUMN idempotency_request.idempotency_key; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.idempotency_request.idempotency_key IS 'Khóa idempotency do client hoặc hệ thống gửi lên.';


--
-- Name: COLUMN idempotency_request.request_type; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.idempotency_request.request_type IS 'Loại request được bảo vệ idempotency: hold_seats, checkout, payment_callback.';


--
-- Name: COLUMN idempotency_request.customer_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.idempotency_request.customer_id IS 'Người dùng thực hiện request, có thể null với callback hệ thống.';


--
-- Name: COLUMN idempotency_request.request_hash; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.idempotency_request.request_hash IS 'Bản băm nội dung request để đối chiếu cùng khóa nhưng khác payload.';


--
-- Name: COLUMN idempotency_request.status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.idempotency_request.status IS 'Trạng thái xử lý idempotency: processing, completed, failed, expired.';


--
-- Name: COLUMN idempotency_request.response_snapshot; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.idempotency_request.response_snapshot IS 'Snapshot phản hồi để trả lại nếu request bị gọi lặp.';


--
-- Name: COLUMN idempotency_request.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.idempotency_request.created_at IS 'Thời điểm tạo bản ghi idempotency.';


--
-- Name: COLUMN idempotency_request.expired_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.idempotency_request.expired_at IS 'Thời điểm hết hiệu lực của bản ghi idempotency.';


--
-- Name: idempotency_request_idempotency_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.idempotency_request_idempotency_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: idempotency_request_idempotency_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.idempotency_request_idempotency_id_seq OWNED BY ticketing.idempotency_request.idempotency_id;


--
-- Name: payment_callback_log; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.payment_callback_log (
    callback_log_id bigint NOT NULL,
    payment_id bigint,
    payment_provider character varying(30) NOT NULL,
    external_transaction_ref character varying(100),
    callback_signature character varying(500),
    payload text NOT NULL,
    signature_valid boolean NOT NULL,
    processed_status character varying(20) NOT NULL,
    received_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    processed_at timestamp(3) without time zone,
    CONSTRAINT ck_payment_callback_log_provider CHECK (((payment_provider)::text = ANY ((ARRAY['vnpay'::character varying, 'momo'::character varying, 'mock'::character varying])::text[]))),
    CONSTRAINT ck_payment_callback_log_status CHECK (((processed_status)::text = ANY ((ARRAY['received'::character varying, 'processed'::character varying, 'ignored'::character varying, 'failed'::character varying])::text[])))
);


--
-- Name: TABLE payment_callback_log; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.payment_callback_log IS 'Lưu log callback từ cổng thanh toán để debug, đối soát và chống xử lý lặp.';


--
-- Name: COLUMN payment_callback_log.callback_log_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_callback_log.callback_log_id IS 'Khóa chính định danh log callback.';


--
-- Name: COLUMN payment_callback_log.payment_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_callback_log.payment_id IS 'Tham chiếu giao dịch thanh toán đã resolve được, nếu có.';


--
-- Name: COLUMN payment_callback_log.payment_provider; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_callback_log.payment_provider IS 'Cổng thanh toán gửi callback.';


--
-- Name: COLUMN payment_callback_log.external_transaction_ref; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_callback_log.external_transaction_ref IS 'Mã giao dịch ngoài từ cổng thanh toán.';


--
-- Name: COLUMN payment_callback_log.callback_signature; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_callback_log.callback_signature IS 'Chữ ký callback dùng để xác thực.';


--
-- Name: COLUMN payment_callback_log.payload; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_callback_log.payload IS 'Payload raw của callback nhận được.';


--
-- Name: COLUMN payment_callback_log.signature_valid; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_callback_log.signature_valid IS 'Kết quả kiểm tra chữ ký callback.';


--
-- Name: COLUMN payment_callback_log.processed_status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_callback_log.processed_status IS 'Trạng thái xử lý log callback: received, processed, ignored, failed.';


--
-- Name: COLUMN payment_callback_log.received_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_callback_log.received_at IS 'Thời điểm hệ thống nhận callback.';


--
-- Name: COLUMN payment_callback_log.processed_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_callback_log.processed_at IS 'Thời điểm callback được xử lý xong.';


--
-- Name: payment_callback_log_callback_log_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.payment_callback_log_callback_log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: payment_callback_log_callback_log_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.payment_callback_log_callback_log_id_seq OWNED BY ticketing.payment_callback_log.callback_log_id;


--
-- Name: payment_transaction; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.payment_transaction (
    payment_id bigint NOT NULL,
    order_id bigint NOT NULL,
    payment_provider character varying(30) NOT NULL,
    payment_ref character varying(100) NOT NULL,
    provider_transaction_ref character varying(100),
    amount numeric(18,2) NOT NULL,
    payment_status character varying(20) NOT NULL,
    requested_at timestamp(3) without time zone NOT NULL,
    confirmed_at timestamp(3) without time zone,
    raw_request_payload text,
    raw_callback_payload text,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_at timestamp(3) without time zone,
    CONSTRAINT ck_payment_transaction_amount CHECK ((amount > (0)::numeric)),
    CONSTRAINT ck_payment_transaction_provider CHECK (((payment_provider)::text = ANY ((ARRAY['vnpay'::character varying, 'momo'::character varying, 'mock'::character varying])::text[]))),
    CONSTRAINT ck_payment_transaction_status CHECK (((payment_status)::text = ANY ((ARRAY['initiated'::character varying, 'pending'::character varying, 'success'::character varying, 'failed'::character varying, 'cancelled'::character varying])::text[])))
);


--
-- Name: TABLE payment_transaction; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.payment_transaction IS 'Lưu giao dịch thanh toán của đơn hàng, hỗ trợ nhiều lần thử thanh toán nếu cần.';


--
-- Name: COLUMN payment_transaction.payment_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.payment_id IS 'Khóa chính định danh giao dịch thanh toán.';


--
-- Name: COLUMN payment_transaction.order_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.order_id IS 'Tham chiếu đơn hàng cần thanh toán.';


--
-- Name: COLUMN payment_transaction.payment_provider; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.payment_provider IS 'Cổng thanh toán được sử dụng: vnpay, momo, mock.';


--
-- Name: COLUMN payment_transaction.payment_ref; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.payment_ref IS 'Mã giao dịch nội bộ duy nhất của hệ thống.';


--
-- Name: COLUMN payment_transaction.provider_transaction_ref; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.provider_transaction_ref IS 'Mã giao dịch do cổng thanh toán trả về.';


--
-- Name: COLUMN payment_transaction.amount; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.amount IS 'Số tiền giao dịch.';


--
-- Name: COLUMN payment_transaction.payment_status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.payment_status IS 'Trạng thái giao dịch: initiated, pending, success, failed, cancelled.';


--
-- Name: COLUMN payment_transaction.requested_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.requested_at IS 'Thời điểm khởi tạo giao dịch thanh toán.';


--
-- Name: COLUMN payment_transaction.confirmed_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.confirmed_at IS 'Thời điểm xác nhận kết quả giao dịch.';


--
-- Name: COLUMN payment_transaction.raw_request_payload; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.raw_request_payload IS 'Payload request gửi sang cổng thanh toán.';


--
-- Name: COLUMN payment_transaction.raw_callback_payload; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.raw_callback_payload IS 'Payload callback cuối cùng nhận từ cổng thanh toán.';


--
-- Name: COLUMN payment_transaction.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.created_at IS 'Thời điểm tạo bản ghi giao dịch.';


--
-- Name: COLUMN payment_transaction.updated_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.payment_transaction.updated_at IS 'Thời điểm cập nhật giao dịch gần nhất.';


--
-- Name: payment_transaction_payment_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.payment_transaction_payment_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: payment_transaction_payment_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.payment_transaction_payment_id_seq OWNED BY ticketing.payment_transaction.payment_id;


--
-- Name: seat_hold; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.seat_hold (
    hold_id bigint NOT NULL,
    hold_code character varying(50) NOT NULL,
    event_id bigint NOT NULL,
    customer_id bigint NOT NULL,
    status character varying(20) NOT NULL,
    hold_started_at timestamp(3) without time zone NOT NULL,
    hold_expires_at timestamp(3) without time zone NOT NULL,
    released_at timestamp(3) without time zone,
    release_reason character varying(100),
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    CONSTRAINT ck_seat_hold_status CHECK (((status)::text = ANY ((ARRAY['active'::character varying, 'expired'::character varying, 'released'::character varying, 'converted'::character varying, 'cancelled'::character varying])::text[]))),
    CONSTRAINT ck_seat_hold_time_range CHECK ((hold_expires_at > hold_started_at))
);


--
-- Name: TABLE seat_hold; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.seat_hold IS 'Header phiên giữ ghế của người dùng trong thời hạn tạm giữ, mặc định 10 phút.';


--
-- Name: COLUMN seat_hold.hold_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold.hold_id IS 'Khóa chính định danh phiên giữ ghế.';


--
-- Name: COLUMN seat_hold.hold_code; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold.hold_code IS 'Mã phiên giữ ghế duy nhất.';


--
-- Name: COLUMN seat_hold.event_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold.event_id IS 'Tham chiếu sự kiện được giữ ghế.';


--
-- Name: COLUMN seat_hold.customer_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold.customer_id IS 'Người dùng đang giữ ghế.';


--
-- Name: COLUMN seat_hold.status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold.status IS 'Trạng thái hold: active, expired, released, converted, cancelled.';


--
-- Name: COLUMN seat_hold.hold_started_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold.hold_started_at IS 'Thời điểm bắt đầu giữ ghế.';


--
-- Name: COLUMN seat_hold.hold_expires_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold.hold_expires_at IS 'Thời điểm hết hạn giữ ghế.';


--
-- Name: COLUMN seat_hold.released_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold.released_at IS 'Thời điểm hold được giải phóng nếu có.';


--
-- Name: COLUMN seat_hold.release_reason; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold.release_reason IS 'Lý do giải phóng hoặc hết hiệu lực hold.';


--
-- Name: COLUMN seat_hold.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold.created_at IS 'Thời điểm tạo bản ghi hold.';


--
-- Name: seat_hold_hold_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.seat_hold_hold_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: seat_hold_hold_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.seat_hold_hold_id_seq OWNED BY ticketing.seat_hold.hold_id;


--
-- Name: seat_hold_item; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.seat_hold_item (
    hold_item_id bigint NOT NULL,
    hold_id bigint NOT NULL,
    event_seat_inventory_id bigint NOT NULL,
    seat_id bigint NOT NULL,
    zone_id bigint NOT NULL,
    price_at_hold numeric(18,2) NOT NULL,
    seat_label_snapshot character varying(50),
    zone_name_snapshot character varying(100),
    status character varying(20) NOT NULL,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    CONSTRAINT ck_seat_hold_item_price CHECK ((price_at_hold >= (0)::numeric)),
    CONSTRAINT ck_seat_hold_item_status CHECK (((status)::text = ANY ((ARRAY['active'::character varying, 'released'::character varying, 'converted'::character varying, 'expired'::character varying])::text[])))
);


--
-- Name: TABLE seat_hold_item; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.seat_hold_item IS 'Chi tiết từng ghế thuộc một phiên giữ ghế.';


--
-- Name: COLUMN seat_hold_item.hold_item_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold_item.hold_item_id IS 'Khóa chính định danh dòng chi tiết hold.';


--
-- Name: COLUMN seat_hold_item.hold_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold_item.hold_id IS 'Tham chiếu phiên giữ ghế.';


--
-- Name: COLUMN seat_hold_item.event_seat_inventory_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold_item.event_seat_inventory_id IS 'Tham chiếu inventory ghế theo event.';


--
-- Name: COLUMN seat_hold_item.seat_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold_item.seat_id IS 'Tham chiếu ghế master của venue.';


--
-- Name: COLUMN seat_hold_item.zone_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold_item.zone_id IS 'Tham chiếu zone của event tại thời điểm giữ ghế.';


--
-- Name: COLUMN seat_hold_item.price_at_hold; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold_item.price_at_hold IS 'Giá ghế tại thời điểm hold.';


--
-- Name: COLUMN seat_hold_item.seat_label_snapshot; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold_item.seat_label_snapshot IS 'Nhãn ghế snapshot tại thời điểm hold.';


--
-- Name: COLUMN seat_hold_item.zone_name_snapshot; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold_item.zone_name_snapshot IS 'Tên zone snapshot tại thời điểm hold.';


--
-- Name: COLUMN seat_hold_item.status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold_item.status IS 'Trạng thái dòng hold item: active, released, converted, expired.';


--
-- Name: COLUMN seat_hold_item.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.seat_hold_item.created_at IS 'Thời điểm tạo dòng chi tiết hold.';


--
-- Name: seat_hold_item_hold_item_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.seat_hold_item_hold_item_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: seat_hold_item_hold_item_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.seat_hold_item_hold_item_id_seq OWNED BY ticketing.seat_hold_item.hold_item_id;


--
-- Name: sys_role; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.sys_role (
    role_id bigint NOT NULL,
    role_code character varying(50) NOT NULL,
    role_name character varying(100) NOT NULL,
    description character varying(500),
    status character varying(20) NOT NULL,
    created_by bigint,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_by bigint,
    updated_at timestamp(3) without time zone,
    is_deleted boolean DEFAULT false NOT NULL,
    CONSTRAINT ck_sys_role_status CHECK (((status)::text = ANY ((ARRAY['active'::character varying, 'inactive'::character varying])::text[])))
);


--
-- Name: TABLE sys_role; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.sys_role IS 'Danh mục vai trò dùng cho phân quyền hệ thống.';


--
-- Name: COLUMN sys_role.role_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_role.role_id IS 'Khóa chính định danh vai trò.';


--
-- Name: COLUMN sys_role.role_code; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_role.role_code IS 'Mã vai trò duy nhất, ví dụ SUPER_ADMIN, EVENT_ADMIN, CUSTOMER.';


--
-- Name: COLUMN sys_role.role_name; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_role.role_name IS 'Tên hiển thị của vai trò.';


--
-- Name: COLUMN sys_role.description; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_role.description IS 'Mô tả vai trò.';


--
-- Name: COLUMN sys_role.status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_role.status IS 'Trạng thái vai trò: active hoặc inactive.';


--
-- Name: COLUMN sys_role.created_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_role.created_by IS 'Người tạo bản ghi.';


--
-- Name: COLUMN sys_role.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_role.created_at IS 'Thời điểm tạo bản ghi.';


--
-- Name: COLUMN sys_role.updated_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_role.updated_by IS 'Người cập nhật bản ghi gần nhất.';


--
-- Name: COLUMN sys_role.updated_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_role.updated_at IS 'Thời điểm cập nhật gần nhất.';


--
-- Name: COLUMN sys_role.is_deleted; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_role.is_deleted IS 'Đánh dấu xóa mềm bản ghi.';


--
-- Name: sys_role_role_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.sys_role_role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: sys_role_role_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.sys_role_role_id_seq OWNED BY ticketing.sys_role.role_id;


--
-- Name: sys_user; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.sys_user (
    user_id bigint NOT NULL,
    username character varying(50) NOT NULL,
    email character varying(255),
    phone character varying(20),
    password_hash character varying(500) NOT NULL,
    full_name character varying(255) NOT NULL,
    user_type character varying(20) NOT NULL,
    status character varying(20) NOT NULL,
    last_login_at timestamp(3) without time zone,
    created_by bigint,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_by bigint,
    updated_at timestamp(3) without time zone,
    is_deleted boolean DEFAULT false NOT NULL,
    CONSTRAINT ck_sys_user_status CHECK (((status)::text = ANY ((ARRAY['active'::character varying, 'inactive'::character varying, 'locked'::character varying])::text[]))),
    CONSTRAINT ck_sys_user_user_type CHECK (((user_type)::text = ANY ((ARRAY['admin'::character varying, 'customer'::character varying])::text[])))
);


--
-- Name: TABLE sys_user; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.sys_user IS 'Lưu tài khoản người dùng của hệ thống, gồm admin và customer.';


--
-- Name: COLUMN sys_user.user_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.user_id IS 'Khóa chính định danh người dùng.';


--
-- Name: COLUMN sys_user.username; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.username IS 'Tên đăng nhập duy nhất của người dùng.';


--
-- Name: COLUMN sys_user.email; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.email IS 'Địa chỉ email của người dùng.';


--
-- Name: COLUMN sys_user.phone; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.phone IS 'Số điện thoại của người dùng.';


--
-- Name: COLUMN sys_user.password_hash; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.password_hash IS 'Mật khẩu đã băm, không lưu plain text.';


--
-- Name: COLUMN sys_user.full_name; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.full_name IS 'Họ tên đầy đủ của người dùng.';


--
-- Name: COLUMN sys_user.user_type; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.user_type IS 'Loại người dùng: admin hoặc customer.';


--
-- Name: COLUMN sys_user.status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.status IS 'Trạng thái tài khoản: active, inactive, locked.';


--
-- Name: COLUMN sys_user.last_login_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.last_login_at IS 'Thời điểm đăng nhập gần nhất.';


--
-- Name: COLUMN sys_user.created_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.created_by IS 'Người tạo bản ghi.';


--
-- Name: COLUMN sys_user.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.created_at IS 'Thời điểm tạo bản ghi.';


--
-- Name: COLUMN sys_user.updated_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.updated_by IS 'Người cập nhật bản ghi gần nhất.';


--
-- Name: COLUMN sys_user.updated_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.updated_at IS 'Thời điểm cập nhật gần nhất.';


--
-- Name: COLUMN sys_user.is_deleted; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user.is_deleted IS 'Đánh dấu xóa mềm bản ghi.';


--
-- Name: sys_user_role; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.sys_user_role (
    user_role_id bigint NOT NULL,
    user_id bigint NOT NULL,
    role_id bigint NOT NULL,
    assigned_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    assigned_by bigint,
    is_deleted boolean DEFAULT false NOT NULL
);


--
-- Name: TABLE sys_user_role; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.sys_user_role IS 'Bảng gán vai trò cho người dùng theo quan hệ nhiều-nhiều.';


--
-- Name: COLUMN sys_user_role.user_role_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user_role.user_role_id IS 'Khóa chính định danh bản ghi gán vai trò.';


--
-- Name: COLUMN sys_user_role.user_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user_role.user_id IS 'Tham chiếu người dùng được gán vai trò.';


--
-- Name: COLUMN sys_user_role.role_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user_role.role_id IS 'Tham chiếu vai trò được gán.';


--
-- Name: COLUMN sys_user_role.assigned_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user_role.assigned_at IS 'Thời điểm vai trò được gán cho người dùng.';


--
-- Name: COLUMN sys_user_role.assigned_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user_role.assigned_by IS 'Người thực hiện gán vai trò.';


--
-- Name: COLUMN sys_user_role.is_deleted; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.sys_user_role.is_deleted IS 'Đánh dấu xóa mềm bản ghi gán vai trò.';


--
-- Name: sys_user_role_user_role_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.sys_user_role_user_role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: sys_user_role_user_role_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.sys_user_role_user_role_id_seq OWNED BY ticketing.sys_user_role.user_role_id;


--
-- Name: sys_user_user_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.sys_user_user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: sys_user_user_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.sys_user_user_id_seq OWNED BY ticketing.sys_user.user_id;


--
-- Name: ticket; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.ticket (
    ticket_id bigint NOT NULL,
    ticket_code character varying(100) NOT NULL,
    order_item_id bigint NOT NULL,
    event_id bigint NOT NULL,
    customer_id bigint NOT NULL,
    seat_id bigint NOT NULL,
    seat_label_snapshot character varying(50),
    zone_name_snapshot character varying(100),
    event_name_snapshot character varying(255),
    ticket_status character varying(20) NOT NULL,
    issued_at timestamp(3) without time zone NOT NULL,
    checked_in_at timestamp(3) without time zone,
    CONSTRAINT ck_ticket_status CHECK (((ticket_status)::text = ANY ((ARRAY['active'::character varying, 'used'::character varying, 'cancelled'::character varying, 'refunded'::character varying])::text[])))
);


--
-- Name: TABLE ticket; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.ticket IS 'Vé phát hành cho từng ghế sau khi đơn hàng được thanh toán thành công.';


--
-- Name: COLUMN ticket.ticket_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.ticket_id IS 'Khóa chính định danh vé.';


--
-- Name: COLUMN ticket.ticket_code; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.ticket_code IS 'Mã vé duy nhất.';


--
-- Name: COLUMN ticket.order_item_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.order_item_id IS 'Tham chiếu dòng chi tiết đơn hàng đã phát hành vé.';


--
-- Name: COLUMN ticket.event_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.event_id IS 'Tham chiếu sự kiện của vé.';


--
-- Name: COLUMN ticket.customer_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.customer_id IS 'Người dùng sở hữu vé.';


--
-- Name: COLUMN ticket.seat_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.seat_id IS 'Tham chiếu ghế master của venue.';


--
-- Name: COLUMN ticket.seat_label_snapshot; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.seat_label_snapshot IS 'Nhãn ghế snapshot trên vé.';


--
-- Name: COLUMN ticket.zone_name_snapshot; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.zone_name_snapshot IS 'Tên zone snapshot trên vé.';


--
-- Name: COLUMN ticket.event_name_snapshot; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.event_name_snapshot IS 'Tên sự kiện snapshot trên vé.';


--
-- Name: COLUMN ticket.ticket_status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.ticket_status IS 'Trạng thái vé: active, used, cancelled, refunded.';


--
-- Name: COLUMN ticket.issued_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.issued_at IS 'Thời điểm vé được phát hành.';


--
-- Name: COLUMN ticket.checked_in_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket.checked_in_at IS 'Thời điểm vé được check-in nếu có.';


--
-- Name: ticket_order; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.ticket_order (
    order_id bigint NOT NULL,
    order_code character varying(50) NOT NULL,
    event_id bigint NOT NULL,
    customer_id bigint NOT NULL,
    hold_id bigint,
    total_amount numeric(18,2) NOT NULL,
    discount_amount numeric(18,2) DEFAULT 0 NOT NULL,
    final_amount numeric(18,2) NOT NULL,
    order_status character varying(30) NOT NULL,
    expired_at timestamp(3) without time zone,
    paid_at timestamp(3) without time zone,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_at timestamp(3) without time zone,
    CONSTRAINT ck_ticket_order_discount_amount CHECK ((discount_amount >= (0)::numeric)),
    CONSTRAINT ck_ticket_order_final_amount CHECK ((final_amount >= (0)::numeric)),
    CONSTRAINT ck_ticket_order_status CHECK (((order_status)::text = ANY ((ARRAY['pending_payment'::character varying, 'paid'::character varying, 'payment_failed'::character varying, 'cancelled'::character varying, 'expired'::character varying])::text[]))),
    CONSTRAINT ck_ticket_order_total_amount CHECK ((total_amount >= (0)::numeric))
);


--
-- Name: TABLE ticket_order; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.ticket_order IS 'Header đơn hàng vé được tạo từ flow checkout.';


--
-- Name: COLUMN ticket_order.order_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.order_id IS 'Khóa chính định danh đơn hàng.';


--
-- Name: COLUMN ticket_order.order_code; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.order_code IS 'Mã đơn hàng duy nhất.';


--
-- Name: COLUMN ticket_order.event_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.event_id IS 'Tham chiếu sự kiện của đơn hàng.';


--
-- Name: COLUMN ticket_order.customer_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.customer_id IS 'Người dùng sở hữu đơn hàng.';


--
-- Name: COLUMN ticket_order.hold_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.hold_id IS 'Tham chiếu hold đã được convert sang đơn hàng.';


--
-- Name: COLUMN ticket_order.total_amount; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.total_amount IS 'Tổng tiền trước giảm giá.';


--
-- Name: COLUMN ticket_order.discount_amount; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.discount_amount IS 'Tổng giá trị giảm giá của đơn hàng.';


--
-- Name: COLUMN ticket_order.final_amount; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.final_amount IS 'Số tiền cuối cùng cần thanh toán.';


--
-- Name: COLUMN ticket_order.order_status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.order_status IS 'Trạng thái đơn hàng: pending_payment, paid, payment_failed, cancelled, expired.';


--
-- Name: COLUMN ticket_order.expired_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.expired_at IS 'Hạn thanh toán của đơn hàng.';


--
-- Name: COLUMN ticket_order.paid_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.paid_at IS 'Thời điểm thanh toán thành công.';


--
-- Name: COLUMN ticket_order.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.created_at IS 'Thời điểm tạo đơn hàng.';


--
-- Name: COLUMN ticket_order.updated_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order.updated_at IS 'Thời điểm cập nhật đơn hàng gần nhất.';


--
-- Name: ticket_order_item; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.ticket_order_item (
    order_item_id bigint NOT NULL,
    order_id bigint NOT NULL,
    event_seat_inventory_id bigint NOT NULL,
    seat_id bigint NOT NULL,
    zone_id bigint NOT NULL,
    unit_price numeric(18,2) NOT NULL,
    seat_label_snapshot character varying(50),
    zone_name_snapshot character varying(100),
    item_status character varying(20) NOT NULL,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    CONSTRAINT ck_ticket_order_item_status CHECK (((item_status)::text = ANY ((ARRAY['pending'::character varying, 'paid'::character varying, 'cancelled'::character varying])::text[]))),
    CONSTRAINT ck_ticket_order_item_unit_price CHECK ((unit_price >= (0)::numeric))
);


--
-- Name: TABLE ticket_order_item; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.ticket_order_item IS 'Chi tiết từng ghế được mua trong một đơn hàng.';


--
-- Name: COLUMN ticket_order_item.order_item_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order_item.order_item_id IS 'Khóa chính định danh dòng chi tiết đơn hàng.';


--
-- Name: COLUMN ticket_order_item.order_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order_item.order_id IS 'Tham chiếu đơn hàng.';


--
-- Name: COLUMN ticket_order_item.event_seat_inventory_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order_item.event_seat_inventory_id IS 'Tham chiếu inventory ghế theo event.';


--
-- Name: COLUMN ticket_order_item.seat_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order_item.seat_id IS 'Tham chiếu ghế master của venue.';


--
-- Name: COLUMN ticket_order_item.zone_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order_item.zone_id IS 'Tham chiếu zone tại thời điểm đặt mua.';


--
-- Name: COLUMN ticket_order_item.unit_price; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order_item.unit_price IS 'Đơn giá của ghế tại thời điểm checkout.';


--
-- Name: COLUMN ticket_order_item.seat_label_snapshot; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order_item.seat_label_snapshot IS 'Nhãn ghế snapshot tại thời điểm checkout.';


--
-- Name: COLUMN ticket_order_item.zone_name_snapshot; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order_item.zone_name_snapshot IS 'Tên zone snapshot tại thời điểm checkout.';


--
-- Name: COLUMN ticket_order_item.item_status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order_item.item_status IS 'Trạng thái dòng chi tiết đơn hàng: pending, paid, cancelled.';


--
-- Name: COLUMN ticket_order_item.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.ticket_order_item.created_at IS 'Thời điểm tạo dòng chi tiết đơn hàng.';


--
-- Name: ticket_order_item_order_item_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.ticket_order_item_order_item_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: ticket_order_item_order_item_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.ticket_order_item_order_item_id_seq OWNED BY ticketing.ticket_order_item.order_item_id;


--
-- Name: ticket_order_order_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.ticket_order_order_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: ticket_order_order_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.ticket_order_order_id_seq OWNED BY ticketing.ticket_order.order_id;


--
-- Name: ticket_ticket_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.ticket_ticket_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: ticket_ticket_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.ticket_ticket_id_seq OWNED BY ticketing.ticket.ticket_id;


--
-- Name: venue; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.venue (
    venue_id bigint NOT NULL,
    venue_code character varying(50) NOT NULL,
    venue_name character varying(255) NOT NULL,
    address_line character varying(500),
    city character varying(100),
    country character varying(100),
    status character varying(20) NOT NULL,
    created_by bigint,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_by bigint,
    updated_at timestamp(3) without time zone,
    is_deleted boolean DEFAULT false NOT NULL,
    CONSTRAINT ck_venue_status CHECK (((status)::text = ANY (ARRAY[('active'::character varying)::text, ('inactive'::character varying)::text])))
);


--
-- Name: venue_seat; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.venue_seat (
    seat_id bigint NOT NULL,
    venue_id bigint NOT NULL,
    section_id bigint NOT NULL,
    seat_code character varying(50) NOT NULL,
    row_label character varying(20),
    seat_number character varying(20),
    seat_label character varying(50),
    x_pos numeric(10,2),
    y_pos numeric(10,2),
    seat_type character varying(20) NOT NULL,
    status character varying(20) NOT NULL,
    created_by bigint,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_by bigint,
    updated_at timestamp(3) without time zone,
    is_deleted boolean DEFAULT false NOT NULL,
    CONSTRAINT ck_venue_seat_seat_type CHECK (((seat_type)::text = ANY ((ARRAY['seat'::character varying, 'standing'::character varying, 'table'::character varying])::text[]))),
    CONSTRAINT ck_venue_seat_status CHECK (((status)::text = ANY ((ARRAY['active'::character varying, 'inactive'::character varying])::text[])))
);


--
-- Name: TABLE venue_seat; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.venue_seat IS 'Danh sách ghế gốc của venue, dùng làm seat master và tái sử dụng cho nhiều event.';


--
-- Name: COLUMN venue_seat.seat_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.seat_id IS 'Khóa chính định danh ghế.';


--
-- Name: COLUMN venue_seat.venue_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.venue_id IS 'Tham chiếu địa điểm chứa ghế.';


--
-- Name: COLUMN venue_seat.section_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.section_id IS 'Tham chiếu section chứa ghế.';


--
-- Name: COLUMN venue_seat.seat_code; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.seat_code IS 'Mã ghế duy nhất trong phạm vi venue.';


--
-- Name: COLUMN venue_seat.row_label; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.row_label IS 'Ký hiệu hàng ghế, ví dụ A, B, C.';


--
-- Name: COLUMN venue_seat.seat_number; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.seat_number IS 'Số ghế trong hàng.';


--
-- Name: COLUMN venue_seat.seat_label; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.seat_label IS 'Nhãn hiển thị của ghế, ví dụ A01.';


--
-- Name: COLUMN venue_seat.x_pos; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.x_pos IS 'Tọa độ X của ghế trên seat map.';


--
-- Name: COLUMN venue_seat.y_pos; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.y_pos IS 'Tọa độ Y của ghế trên seat map.';


--
-- Name: COLUMN venue_seat.seat_type; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.seat_type IS 'Loại vị trí: seat, standing, table.';


--
-- Name: COLUMN venue_seat.status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.status IS 'Trạng thái ghế master: active hoặc inactive.';


--
-- Name: COLUMN venue_seat.created_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.created_by IS 'Người tạo bản ghi.';


--
-- Name: COLUMN venue_seat.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.created_at IS 'Thời điểm tạo bản ghi.';


--
-- Name: COLUMN venue_seat.updated_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.updated_by IS 'Người cập nhật bản ghi gần nhất.';


--
-- Name: COLUMN venue_seat.updated_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.updated_at IS 'Thời điểm cập nhật gần nhất.';


--
-- Name: COLUMN venue_seat.is_deleted; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_seat.is_deleted IS 'Đánh dấu xóa mềm bản ghi.';


--
-- Name: venue_seat_seat_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.venue_seat_seat_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: venue_seat_seat_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.venue_seat_seat_id_seq OWNED BY ticketing.venue_seat.seat_id;


--
-- Name: venue_section; Type: TABLE; Schema: ticketing; Owner: -
--

CREATE TABLE ticketing.venue_section (
    section_id bigint NOT NULL,
    venue_id bigint NOT NULL,
    section_code character varying(50) NOT NULL,
    section_name character varying(255) NOT NULL,
    display_order integer DEFAULT 0 NOT NULL,
    status character varying(20) NOT NULL,
    created_by bigint,
    created_at timestamp(3) without time zone DEFAULT now() NOT NULL,
    updated_by bigint,
    updated_at timestamp(3) without time zone,
    is_deleted boolean DEFAULT false NOT NULL,
    CONSTRAINT ck_venue_section_status CHECK (((status)::text = ANY ((ARRAY['active'::character varying, 'inactive'::character varying])::text[])))
);


--
-- Name: TABLE venue_section; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON TABLE ticketing.venue_section IS 'Danh mục khu vực/section bên trong một địa điểm tổ chức.';


--
-- Name: COLUMN venue_section.section_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.section_id IS 'Khóa chính định danh section.';


--
-- Name: COLUMN venue_section.venue_id; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.venue_id IS 'Tham chiếu địa điểm chứa section.';


--
-- Name: COLUMN venue_section.section_code; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.section_code IS 'Mã section duy nhất trong phạm vi venue.';


--
-- Name: COLUMN venue_section.section_name; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.section_name IS 'Tên hiển thị của section.';


--
-- Name: COLUMN venue_section.display_order; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.display_order IS 'Thứ tự hiển thị của section trên admin hoặc seat map.';


--
-- Name: COLUMN venue_section.status; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.status IS 'Trạng thái section: active hoặc inactive.';


--
-- Name: COLUMN venue_section.created_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.created_by IS 'Người tạo bản ghi.';


--
-- Name: COLUMN venue_section.created_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.created_at IS 'Thời điểm tạo bản ghi.';


--
-- Name: COLUMN venue_section.updated_by; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.updated_by IS 'Người cập nhật bản ghi gần nhất.';


--
-- Name: COLUMN venue_section.updated_at; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.updated_at IS 'Thời điểm cập nhật gần nhất.';


--
-- Name: COLUMN venue_section.is_deleted; Type: COMMENT; Schema: ticketing; Owner: -
--

COMMENT ON COLUMN ticketing.venue_section.is_deleted IS 'Đánh dấu xóa mềm bản ghi.';


--
-- Name: venue_section_section_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.venue_section_section_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: venue_section_section_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.venue_section_section_id_seq OWNED BY ticketing.venue_section.section_id;


--
-- Name: venue_venue_id_seq; Type: SEQUENCE; Schema: ticketing; Owner: -
--

CREATE SEQUENCE ticketing.venue_venue_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: venue_venue_id_seq; Type: SEQUENCE OWNED BY; Schema: ticketing; Owner: -
--

ALTER SEQUENCE ticketing.venue_venue_id_seq OWNED BY ticketing.venue.venue_id;


--
-- Name: audit_log audit_log_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.audit_log ALTER COLUMN audit_log_id SET DEFAULT nextval('ticketing.audit_log_audit_log_id_seq'::regclass);


--
-- Name: customer customer_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.customer ALTER COLUMN customer_id SET DEFAULT nextval('ticketing.customer_customer_id_seq'::regclass);


--
-- Name: event event_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event ALTER COLUMN event_id SET DEFAULT nextval('ticketing.event_event_id_seq'::regclass);


--
-- Name: event_publish_log event_publish_log_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_publish_log ALTER COLUMN event_publish_log_id SET DEFAULT nextval('ticketing.event_publish_log_event_publish_log_id_seq'::regclass);


--
-- Name: event_seat_inventory event_seat_inventory_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_seat_inventory ALTER COLUMN event_seat_inventory_id SET DEFAULT nextval('ticketing.event_seat_inventory_event_seat_inventory_id_seq'::regclass);


--
-- Name: event_zone event_zone_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone ALTER COLUMN event_zone_id SET DEFAULT nextval('ticketing.event_zone_event_zone_id_seq'::regclass);


--
-- Name: event_zone_price event_zone_price_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone_price ALTER COLUMN event_zone_price_id SET DEFAULT nextval('ticketing.event_zone_price_event_zone_price_id_seq'::regclass);


--
-- Name: event_zone_section event_zone_section_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone_section ALTER COLUMN event_zone_section_id SET DEFAULT nextval('ticketing.event_zone_section_event_zone_section_id_seq'::regclass);


--
-- Name: idempotency_request idempotency_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.idempotency_request ALTER COLUMN idempotency_id SET DEFAULT nextval('ticketing.idempotency_request_idempotency_id_seq'::regclass);


--
-- Name: payment_callback_log callback_log_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.payment_callback_log ALTER COLUMN callback_log_id SET DEFAULT nextval('ticketing.payment_callback_log_callback_log_id_seq'::regclass);


--
-- Name: payment_transaction payment_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.payment_transaction ALTER COLUMN payment_id SET DEFAULT nextval('ticketing.payment_transaction_payment_id_seq'::regclass);


--
-- Name: seat_hold hold_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.seat_hold ALTER COLUMN hold_id SET DEFAULT nextval('ticketing.seat_hold_hold_id_seq'::regclass);


--
-- Name: seat_hold_item hold_item_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.seat_hold_item ALTER COLUMN hold_item_id SET DEFAULT nextval('ticketing.seat_hold_item_hold_item_id_seq'::regclass);


--
-- Name: sys_role role_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.sys_role ALTER COLUMN role_id SET DEFAULT nextval('ticketing.sys_role_role_id_seq'::regclass);


--
-- Name: sys_user user_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.sys_user ALTER COLUMN user_id SET DEFAULT nextval('ticketing.sys_user_user_id_seq'::regclass);


--
-- Name: sys_user_role user_role_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.sys_user_role ALTER COLUMN user_role_id SET DEFAULT nextval('ticketing.sys_user_role_user_role_id_seq'::regclass);


--
-- Name: ticket ticket_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket ALTER COLUMN ticket_id SET DEFAULT nextval('ticketing.ticket_ticket_id_seq'::regclass);


--
-- Name: ticket_order order_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order ALTER COLUMN order_id SET DEFAULT nextval('ticketing.ticket_order_order_id_seq'::regclass);


--
-- Name: ticket_order_item order_item_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order_item ALTER COLUMN order_item_id SET DEFAULT nextval('ticketing.ticket_order_item_order_item_id_seq'::regclass);


--
-- Name: venue venue_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue ALTER COLUMN venue_id SET DEFAULT nextval('ticketing.venue_venue_id_seq'::regclass);


--
-- Name: venue_seat seat_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue_seat ALTER COLUMN seat_id SET DEFAULT nextval('ticketing.venue_seat_seat_id_seq'::regclass);


--
-- Name: venue_section section_id; Type: DEFAULT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue_section ALTER COLUMN section_id SET DEFAULT nextval('ticketing.venue_section_section_id_seq'::regclass);


--
-- Name: audit_log audit_log_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.audit_log
    ADD CONSTRAINT audit_log_pkey PRIMARY KEY (audit_log_id);


--
-- Name: customer customer_customer_code_key; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.customer
    ADD CONSTRAINT customer_customer_code_key UNIQUE (customer_code);


--
-- Name: customer customer_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.customer
    ADD CONSTRAINT customer_pkey PRIMARY KEY (customer_id);


--
-- Name: customer customer_username_key; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.customer
    ADD CONSTRAINT customer_username_key UNIQUE (username);


--
-- Name: event event_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event
    ADD CONSTRAINT event_pkey PRIMARY KEY (event_id);


--
-- Name: event_publish_log event_publish_log_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_publish_log
    ADD CONSTRAINT event_publish_log_pkey PRIMARY KEY (event_publish_log_id);


--
-- Name: event_seat_inventory event_seat_inventory_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_seat_inventory
    ADD CONSTRAINT event_seat_inventory_pkey PRIMARY KEY (event_seat_inventory_id);


--
-- Name: event_zone event_zone_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone
    ADD CONSTRAINT event_zone_pkey PRIMARY KEY (event_zone_id);


--
-- Name: event_zone_price event_zone_price_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone_price
    ADD CONSTRAINT event_zone_price_pkey PRIMARY KEY (event_zone_price_id);


--
-- Name: event_zone_section event_zone_section_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone_section
    ADD CONSTRAINT event_zone_section_pkey PRIMARY KEY (event_zone_section_id);


--
-- Name: idempotency_request idempotency_request_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.idempotency_request
    ADD CONSTRAINT idempotency_request_pkey PRIMARY KEY (idempotency_id);


--
-- Name: payment_callback_log payment_callback_log_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.payment_callback_log
    ADD CONSTRAINT payment_callback_log_pkey PRIMARY KEY (callback_log_id);


--
-- Name: payment_transaction payment_transaction_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.payment_transaction
    ADD CONSTRAINT payment_transaction_pkey PRIMARY KEY (payment_id);


--
-- Name: seat_hold_item seat_hold_item_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.seat_hold_item
    ADD CONSTRAINT seat_hold_item_pkey PRIMARY KEY (hold_item_id);


--
-- Name: seat_hold seat_hold_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.seat_hold
    ADD CONSTRAINT seat_hold_pkey PRIMARY KEY (hold_id);


--
-- Name: sys_role sys_role_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.sys_role
    ADD CONSTRAINT sys_role_pkey PRIMARY KEY (role_id);


--
-- Name: sys_user sys_user_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.sys_user
    ADD CONSTRAINT sys_user_pkey PRIMARY KEY (user_id);


--
-- Name: sys_user_role sys_user_role_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.sys_user_role
    ADD CONSTRAINT sys_user_role_pkey PRIMARY KEY (user_role_id);


--
-- Name: ticket_order_item ticket_order_item_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order_item
    ADD CONSTRAINT ticket_order_item_pkey PRIMARY KEY (order_item_id);


--
-- Name: ticket_order ticket_order_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order
    ADD CONSTRAINT ticket_order_pkey PRIMARY KEY (order_id);


--
-- Name: ticket ticket_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket
    ADD CONSTRAINT ticket_pkey PRIMARY KEY (ticket_id);


--
-- Name: venue venue_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue
    ADD CONSTRAINT venue_pkey PRIMARY KEY (venue_id);


--
-- Name: venue_seat venue_seat_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue_seat
    ADD CONSTRAINT venue_seat_pkey PRIMARY KEY (seat_id);


--
-- Name: venue_section venue_section_pkey; Type: CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue_section
    ADD CONSTRAINT venue_section_pkey PRIMARY KEY (section_id);


--
-- Name: idx_customer_email; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX idx_customer_email ON ticketing.customer USING btree (email) WHERE ((is_deleted = false) AND (email IS NOT NULL));


--
-- Name: idx_customer_status; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX idx_customer_status ON ticketing.customer USING btree (status);


--
-- Name: idx_customer_username; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX idx_customer_username ON ticketing.customer USING btree (username) WHERE (is_deleted = false);


--
-- Name: ix_audit_log_actor_user_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_audit_log_actor_user_id ON ticketing.audit_log USING btree (actor_user_id);


--
-- Name: ix_audit_log_entity_lookup; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_audit_log_entity_lookup ON ticketing.audit_log USING btree (entity_name, entity_id, created_at DESC);


--
-- Name: ix_audit_log_module_created_at_desc; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_audit_log_module_created_at_desc ON ticketing.audit_log USING btree (module_name, created_at DESC);


--
-- Name: ix_event_client_homepage_featured; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_client_homepage_featured ON ticketing.event USING btree (is_deleted, is_featured, status, display_order, start_time, event_id);


--
-- Name: ix_event_client_homepage_trending; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_client_homepage_trending ON ticketing.event USING btree (is_deleted, is_trending, status, display_order, start_time, event_id);


--
-- Name: ix_event_client_homepage_upcoming; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_client_homepage_upcoming ON ticketing.event USING btree (is_deleted, status, start_time, is_featured, is_trending, display_order, event_id);


--
-- Name: ix_event_display_order; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_display_order ON ticketing.event USING btree (display_order);


--
-- Name: ix_event_featured_trending; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_featured_trending ON ticketing.event USING btree (is_featured, is_trending);


--
-- Name: ix_event_publish_log_event_changed_at_desc; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_publish_log_event_changed_at_desc ON ticketing.event_publish_log USING btree (event_id, changed_at DESC);


--
-- Name: ix_event_publish_log_event_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_publish_log_event_id ON ticketing.event_publish_log USING btree (event_id);


--
-- Name: ix_event_seat_inventory_current_hold_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_seat_inventory_current_hold_id ON ticketing.event_seat_inventory USING btree (current_hold_id);


--
-- Name: ix_event_seat_inventory_current_order_item_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_seat_inventory_current_order_item_id ON ticketing.event_seat_inventory USING btree (current_order_item_id);


--
-- Name: ix_event_seat_inventory_event_status; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_seat_inventory_event_status ON ticketing.event_seat_inventory USING btree (event_id, seat_status);


--
-- Name: ix_event_seat_inventory_event_zone_status; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_seat_inventory_event_zone_status ON ticketing.event_seat_inventory USING btree (event_id, event_zone_id, seat_status);


--
-- Name: ix_event_start_end_time; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_start_end_time ON ticketing.event USING btree (start_time, end_time);


--
-- Name: ix_event_status; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_status ON ticketing.event USING btree (status);


--
-- Name: ix_event_status_sale_time; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_status_sale_time ON ticketing.event USING btree (status, sale_start_time, sale_end_time);


--
-- Name: ix_event_venue_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_venue_id ON ticketing.event USING btree (venue_id);


--
-- Name: ix_event_zone_event_display_order; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_zone_event_display_order ON ticketing.event_zone USING btree (event_id, display_order);


--
-- Name: ix_event_zone_event_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_zone_event_id ON ticketing.event_zone USING btree (event_id);


--
-- Name: ix_event_zone_price_active_deleted; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_zone_price_active_deleted ON ticketing.event_zone_price USING btree (event_zone_id, is_active, is_deleted);


--
-- Name: ix_event_zone_price_event_zone_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_zone_price_event_zone_id ON ticketing.event_zone_price USING btree (event_zone_id);


--
-- Name: ix_event_zone_price_time_range; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_zone_price_time_range ON ticketing.event_zone_price USING btree (start_time, end_time);


--
-- Name: ix_event_zone_section_event_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_zone_section_event_id ON ticketing.event_zone_section USING btree (event_id);


--
-- Name: ix_event_zone_section_event_zone_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_zone_section_event_zone_id ON ticketing.event_zone_section USING btree (event_zone_id);


--
-- Name: ix_event_zone_section_event_zone_section; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_zone_section_event_zone_section ON ticketing.event_zone_section USING btree (event_id, event_zone_id, section_id);


--
-- Name: ix_event_zone_section_section_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_event_zone_section_section_id ON ticketing.event_zone_section USING btree (section_id);


--
-- Name: ix_idempotency_request_customer_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_idempotency_request_customer_id ON ticketing.idempotency_request USING btree (customer_id);


--
-- Name: ix_idempotency_request_status_expired_at; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_idempotency_request_status_expired_at ON ticketing.idempotency_request USING btree (status, expired_at);


--
-- Name: ix_payment_callback_log_external_transaction_ref; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_payment_callback_log_external_transaction_ref ON ticketing.payment_callback_log USING btree (external_transaction_ref);


--
-- Name: ix_payment_callback_log_payment_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_payment_callback_log_payment_id ON ticketing.payment_callback_log USING btree (payment_id);


--
-- Name: ix_payment_callback_log_provider_external_ref; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_payment_callback_log_provider_external_ref ON ticketing.payment_callback_log USING btree (payment_provider, external_transaction_ref);


--
-- Name: ix_payment_callback_log_status_received_at; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_payment_callback_log_status_received_at ON ticketing.payment_callback_log USING btree (processed_status, received_at);


--
-- Name: ix_payment_transaction_order_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_payment_transaction_order_id ON ticketing.payment_transaction USING btree (order_id);


--
-- Name: ix_payment_transaction_order_status; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_payment_transaction_order_status ON ticketing.payment_transaction USING btree (order_id, payment_status);


--
-- Name: ix_payment_transaction_provider_transaction_ref; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_payment_transaction_provider_transaction_ref ON ticketing.payment_transaction USING btree (provider_transaction_ref);


--
-- Name: ix_seat_hold_customer_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_seat_hold_customer_id ON ticketing.seat_hold USING btree (customer_id);


--
-- Name: ix_seat_hold_event_customer_status; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_seat_hold_event_customer_status ON ticketing.seat_hold USING btree (event_id, customer_id, status);


--
-- Name: ix_seat_hold_event_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_seat_hold_event_id ON ticketing.seat_hold USING btree (event_id);


--
-- Name: ix_seat_hold_event_status_expires_at; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_seat_hold_event_status_expires_at ON ticketing.seat_hold USING btree (event_id, status, hold_expires_at);


--
-- Name: ix_seat_hold_item_hold_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_seat_hold_item_hold_id ON ticketing.seat_hold_item USING btree (hold_id);


--
-- Name: ix_seat_hold_item_inventory_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_seat_hold_item_inventory_id ON ticketing.seat_hold_item USING btree (event_seat_inventory_id);


--
-- Name: ix_seat_hold_item_seat_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_seat_hold_item_seat_id ON ticketing.seat_hold_item USING btree (seat_id);


--
-- Name: ix_seat_hold_item_zone_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_seat_hold_item_zone_id ON ticketing.seat_hold_item USING btree (zone_id);


--
-- Name: ix_seat_hold_status_expires_at; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_seat_hold_status_expires_at ON ticketing.seat_hold USING btree (status, hold_expires_at);


--
-- Name: ix_sys_role_role_code_deleted; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_sys_role_role_code_deleted ON ticketing.sys_role USING btree (role_code, is_deleted);


--
-- Name: ix_sys_role_status; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_sys_role_status ON ticketing.sys_role USING btree (status);


--
-- Name: ix_sys_user_role_role_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_sys_user_role_role_id ON ticketing.sys_user_role USING btree (role_id);


--
-- Name: ix_sys_user_role_user_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_sys_user_role_user_id ON ticketing.sys_user_role USING btree (user_id);


--
-- Name: ix_sys_user_status; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_sys_user_status ON ticketing.sys_user USING btree (status);


--
-- Name: ix_sys_user_user_type; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_sys_user_user_type ON ticketing.sys_user USING btree (user_type);


--
-- Name: ix_sys_user_username_status_deleted; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_sys_user_username_status_deleted ON ticketing.sys_user USING btree (username, status, is_deleted);


--
-- Name: ix_ticket_customer_event; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_customer_event ON ticketing.ticket USING btree (customer_id, event_id);


--
-- Name: ix_ticket_customer_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_customer_id ON ticketing.ticket USING btree (customer_id);


--
-- Name: ix_ticket_event_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_event_id ON ticketing.ticket USING btree (event_id);


--
-- Name: ix_ticket_order_customer_created_at_desc; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_order_customer_created_at_desc ON ticketing.ticket_order USING btree (customer_id, created_at DESC);


--
-- Name: ix_ticket_order_customer_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_order_customer_id ON ticketing.ticket_order USING btree (customer_id);


--
-- Name: ix_ticket_order_event_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_order_event_id ON ticketing.ticket_order USING btree (event_id);


--
-- Name: ix_ticket_order_hold_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_order_hold_id ON ticketing.ticket_order USING btree (hold_id);


--
-- Name: ix_ticket_order_item_inventory_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_order_item_inventory_id ON ticketing.ticket_order_item USING btree (event_seat_inventory_id);


--
-- Name: ix_ticket_order_item_order_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_order_item_order_id ON ticketing.ticket_order_item USING btree (order_id);


--
-- Name: ix_ticket_order_item_seat_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_order_item_seat_id ON ticketing.ticket_order_item USING btree (seat_id);


--
-- Name: ix_ticket_order_item_zone_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_order_item_zone_id ON ticketing.ticket_order_item USING btree (zone_id);


--
-- Name: ix_ticket_order_status_expired_at; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_order_status_expired_at ON ticketing.ticket_order USING btree (order_status, expired_at);


--
-- Name: ix_ticket_status; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_ticket_status ON ticketing.ticket USING btree (ticket_status);


--
-- Name: ix_venue_client_basic_lookup; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_venue_client_basic_lookup ON ticketing.venue USING btree (venue_id, venue_name, city);


--
-- Name: ix_venue_name; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_venue_name ON ticketing.venue USING btree (venue_name);


--
-- Name: ix_venue_seat_section_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_venue_seat_section_id ON ticketing.venue_seat USING btree (section_id);


--
-- Name: ix_venue_seat_venue_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_venue_seat_venue_id ON ticketing.venue_seat USING btree (venue_id);


--
-- Name: ix_venue_seat_venue_section_status_deleted; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_venue_seat_venue_section_status_deleted ON ticketing.venue_seat USING btree (venue_id, section_id, status, is_deleted);


--
-- Name: ix_venue_section_venue_display_order; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_venue_section_venue_display_order ON ticketing.venue_section USING btree (venue_id, display_order);


--
-- Name: ix_venue_section_venue_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_venue_section_venue_id ON ticketing.venue_section USING btree (venue_id);


--
-- Name: ix_venue_status; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE INDEX ix_venue_status ON ticketing.venue USING btree (status);


--
-- Name: ux_event_seat_inventory_event_seat; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_event_seat_inventory_event_seat ON ticketing.event_seat_inventory USING btree (event_id, seat_id);


--
-- Name: ux_event_zone_section_active; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_event_zone_section_active ON ticketing.event_zone_section USING btree (event_id, event_zone_id, section_id) WHERE (is_deleted = false);


--
-- Name: ux_idempotency_request_type_key; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_idempotency_request_type_key ON ticketing.idempotency_request USING btree (request_type, idempotency_key);


--
-- Name: ux_payment_transaction_payment_ref; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_payment_transaction_payment_ref ON ticketing.payment_transaction USING btree (payment_ref);


--
-- Name: ux_payment_transaction_provider_ref; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_payment_transaction_provider_ref ON ticketing.payment_transaction USING btree (provider_transaction_ref) WHERE (provider_transaction_ref IS NOT NULL);


--
-- Name: ux_seat_hold_code; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_seat_hold_code ON ticketing.seat_hold USING btree (hold_code);


--
-- Name: ux_seat_hold_item_hold_inventory; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_seat_hold_item_hold_inventory ON ticketing.seat_hold_item USING btree (hold_id, event_seat_inventory_id);


--
-- Name: ux_sys_role_role_code; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_sys_role_role_code ON ticketing.sys_role USING btree (role_code) WHERE (is_deleted = false);


--
-- Name: ux_sys_user_email; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_sys_user_email ON ticketing.sys_user USING btree (email) WHERE ((email IS NOT NULL) AND (is_deleted = false));


--
-- Name: ux_sys_user_phone; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_sys_user_phone ON ticketing.sys_user USING btree (phone) WHERE ((phone IS NOT NULL) AND (is_deleted = false));


--
-- Name: ux_sys_user_role_user_role; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_sys_user_role_user_role ON ticketing.sys_user_role USING btree (user_id, role_id) WHERE (is_deleted = false);


--
-- Name: ux_sys_user_username; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_sys_user_username ON ticketing.sys_user USING btree (username) WHERE (is_deleted = false);


--
-- Name: ux_ticket_code; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_ticket_code ON ticketing.ticket USING btree (ticket_code);


--
-- Name: ux_ticket_order_code; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_ticket_order_code ON ticketing.ticket_order USING btree (order_code);


--
-- Name: ux_ticket_order_hold_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_ticket_order_hold_id ON ticketing.ticket_order USING btree (hold_id) WHERE (hold_id IS NOT NULL);


--
-- Name: ux_ticket_order_item_id; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_ticket_order_item_id ON ticketing.ticket USING btree (order_item_id);


--
-- Name: ux_ticket_order_item_order_inventory; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_ticket_order_item_order_inventory ON ticketing.ticket_order_item USING btree (order_id, event_seat_inventory_id);


--
-- Name: ux_venue_code; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_venue_code ON ticketing.venue USING btree (venue_code) WHERE (is_deleted = false);


--
-- Name: ux_venue_seat_venue_seat_code; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_venue_seat_venue_seat_code ON ticketing.venue_seat USING btree (venue_id, seat_code) WHERE (is_deleted = false);


--
-- Name: ux_venue_section_venue_code; Type: INDEX; Schema: ticketing; Owner: -
--

CREATE UNIQUE INDEX ux_venue_section_venue_code ON ticketing.venue_section USING btree (venue_id, section_code) WHERE (is_deleted = false);


--
-- Name: audit_log fk_audit_log_actor_user; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.audit_log
    ADD CONSTRAINT fk_audit_log_actor_user FOREIGN KEY (actor_user_id) REFERENCES ticketing.sys_user(user_id);


--
-- Name: event_publish_log fk_event_publish_log_changed_by; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_publish_log
    ADD CONSTRAINT fk_event_publish_log_changed_by FOREIGN KEY (changed_by) REFERENCES ticketing.sys_user(user_id);


--
-- Name: event_publish_log fk_event_publish_log_event; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_publish_log
    ADD CONSTRAINT fk_event_publish_log_event FOREIGN KEY (event_id) REFERENCES ticketing.event(event_id);


--
-- Name: event_seat_inventory fk_event_seat_inventory_current_hold; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_seat_inventory
    ADD CONSTRAINT fk_event_seat_inventory_current_hold FOREIGN KEY (current_hold_id) REFERENCES ticketing.seat_hold(hold_id);


--
-- Name: event_seat_inventory fk_event_seat_inventory_current_order_item; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_seat_inventory
    ADD CONSTRAINT fk_event_seat_inventory_current_order_item FOREIGN KEY (current_order_item_id) REFERENCES ticketing.ticket_order_item(order_item_id);


--
-- Name: event_seat_inventory fk_event_seat_inventory_event; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_seat_inventory
    ADD CONSTRAINT fk_event_seat_inventory_event FOREIGN KEY (event_id) REFERENCES ticketing.event(event_id);


--
-- Name: event_seat_inventory fk_event_seat_inventory_event_zone; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_seat_inventory
    ADD CONSTRAINT fk_event_seat_inventory_event_zone FOREIGN KEY (event_zone_id) REFERENCES ticketing.event_zone(event_zone_id);


--
-- Name: event_seat_inventory fk_event_seat_inventory_seat; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_seat_inventory
    ADD CONSTRAINT fk_event_seat_inventory_seat FOREIGN KEY (seat_id) REFERENCES ticketing.venue_seat(seat_id);


--
-- Name: event_zone_section fk_event_zone_section_created_by; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone_section
    ADD CONSTRAINT fk_event_zone_section_created_by FOREIGN KEY (created_by) REFERENCES ticketing.sys_user(user_id);


--
-- Name: event_zone_section fk_event_zone_section_event; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone_section
    ADD CONSTRAINT fk_event_zone_section_event FOREIGN KEY (event_id) REFERENCES ticketing.event(event_id);


--
-- Name: event_zone_section fk_event_zone_section_event_zone; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone_section
    ADD CONSTRAINT fk_event_zone_section_event_zone FOREIGN KEY (event_zone_id) REFERENCES ticketing.event_zone(event_zone_id);


--
-- Name: event_zone_section fk_event_zone_section_section; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone_section
    ADD CONSTRAINT fk_event_zone_section_section FOREIGN KEY (section_id) REFERENCES ticketing.venue_section(section_id);


--
-- Name: event_zone_section fk_event_zone_section_updated_by; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.event_zone_section
    ADD CONSTRAINT fk_event_zone_section_updated_by FOREIGN KEY (updated_by) REFERENCES ticketing.sys_user(user_id);


--
-- Name: idempotency_request fk_idempotency_request_customer; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.idempotency_request
    ADD CONSTRAINT fk_idempotency_request_customer FOREIGN KEY (customer_id) REFERENCES ticketing.customer(customer_id);


--
-- Name: payment_callback_log fk_payment_callback_log_payment; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.payment_callback_log
    ADD CONSTRAINT fk_payment_callback_log_payment FOREIGN KEY (payment_id) REFERENCES ticketing.payment_transaction(payment_id);


--
-- Name: payment_transaction fk_payment_transaction_order; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.payment_transaction
    ADD CONSTRAINT fk_payment_transaction_order FOREIGN KEY (order_id) REFERENCES ticketing.ticket_order(order_id);


--
-- Name: seat_hold fk_seat_hold_customer; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.seat_hold
    ADD CONSTRAINT fk_seat_hold_customer FOREIGN KEY (customer_id) REFERENCES ticketing.customer(customer_id);


--
-- Name: seat_hold fk_seat_hold_event; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.seat_hold
    ADD CONSTRAINT fk_seat_hold_event FOREIGN KEY (event_id) REFERENCES ticketing.event(event_id);


--
-- Name: seat_hold_item fk_seat_hold_item_hold; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.seat_hold_item
    ADD CONSTRAINT fk_seat_hold_item_hold FOREIGN KEY (hold_id) REFERENCES ticketing.seat_hold(hold_id);


--
-- Name: seat_hold_item fk_seat_hold_item_inventory; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.seat_hold_item
    ADD CONSTRAINT fk_seat_hold_item_inventory FOREIGN KEY (event_seat_inventory_id) REFERENCES ticketing.event_seat_inventory(event_seat_inventory_id);


--
-- Name: seat_hold_item fk_seat_hold_item_seat; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.seat_hold_item
    ADD CONSTRAINT fk_seat_hold_item_seat FOREIGN KEY (seat_id) REFERENCES ticketing.venue_seat(seat_id);


--
-- Name: seat_hold_item fk_seat_hold_item_zone; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.seat_hold_item
    ADD CONSTRAINT fk_seat_hold_item_zone FOREIGN KEY (zone_id) REFERENCES ticketing.event_zone(event_zone_id);


--
-- Name: sys_user_role fk_sys_user_role_assigned_by; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.sys_user_role
    ADD CONSTRAINT fk_sys_user_role_assigned_by FOREIGN KEY (assigned_by) REFERENCES ticketing.sys_user(user_id);


--
-- Name: sys_user_role fk_sys_user_role_role; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.sys_user_role
    ADD CONSTRAINT fk_sys_user_role_role FOREIGN KEY (role_id) REFERENCES ticketing.sys_role(role_id);


--
-- Name: sys_user_role fk_sys_user_role_user; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.sys_user_role
    ADD CONSTRAINT fk_sys_user_role_user FOREIGN KEY (user_id) REFERENCES ticketing.sys_user(user_id);


--
-- Name: ticket fk_ticket_customer; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket
    ADD CONSTRAINT fk_ticket_customer FOREIGN KEY (customer_id) REFERENCES ticketing.customer(customer_id);


--
-- Name: ticket fk_ticket_event; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket
    ADD CONSTRAINT fk_ticket_event FOREIGN KEY (event_id) REFERENCES ticketing.event(event_id);


--
-- Name: ticket_order fk_ticket_order_customer; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order
    ADD CONSTRAINT fk_ticket_order_customer FOREIGN KEY (customer_id) REFERENCES ticketing.customer(customer_id);


--
-- Name: ticket_order fk_ticket_order_event; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order
    ADD CONSTRAINT fk_ticket_order_event FOREIGN KEY (event_id) REFERENCES ticketing.event(event_id);


--
-- Name: ticket_order fk_ticket_order_hold; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order
    ADD CONSTRAINT fk_ticket_order_hold FOREIGN KEY (hold_id) REFERENCES ticketing.seat_hold(hold_id);


--
-- Name: ticket fk_ticket_order_item; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket
    ADD CONSTRAINT fk_ticket_order_item FOREIGN KEY (order_item_id) REFERENCES ticketing.ticket_order_item(order_item_id);


--
-- Name: ticket_order_item fk_ticket_order_item_inventory; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order_item
    ADD CONSTRAINT fk_ticket_order_item_inventory FOREIGN KEY (event_seat_inventory_id) REFERENCES ticketing.event_seat_inventory(event_seat_inventory_id);


--
-- Name: ticket_order_item fk_ticket_order_item_order; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order_item
    ADD CONSTRAINT fk_ticket_order_item_order FOREIGN KEY (order_id) REFERENCES ticketing.ticket_order(order_id);


--
-- Name: ticket_order_item fk_ticket_order_item_seat; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order_item
    ADD CONSTRAINT fk_ticket_order_item_seat FOREIGN KEY (seat_id) REFERENCES ticketing.venue_seat(seat_id);


--
-- Name: ticket_order_item fk_ticket_order_item_zone; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket_order_item
    ADD CONSTRAINT fk_ticket_order_item_zone FOREIGN KEY (zone_id) REFERENCES ticketing.event_zone(event_zone_id);


--
-- Name: ticket fk_ticket_seat; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.ticket
    ADD CONSTRAINT fk_ticket_seat FOREIGN KEY (seat_id) REFERENCES ticketing.venue_seat(seat_id);


--
-- Name: venue fk_venue_created_by; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue
    ADD CONSTRAINT fk_venue_created_by FOREIGN KEY (created_by) REFERENCES ticketing.sys_user(user_id);


--
-- Name: venue_seat fk_venue_seat_created_by; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue_seat
    ADD CONSTRAINT fk_venue_seat_created_by FOREIGN KEY (created_by) REFERENCES ticketing.sys_user(user_id);


--
-- Name: venue_seat fk_venue_seat_section; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue_seat
    ADD CONSTRAINT fk_venue_seat_section FOREIGN KEY (section_id) REFERENCES ticketing.venue_section(section_id);


--
-- Name: venue_seat fk_venue_seat_updated_by; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue_seat
    ADD CONSTRAINT fk_venue_seat_updated_by FOREIGN KEY (updated_by) REFERENCES ticketing.sys_user(user_id);


--
-- Name: venue_section fk_venue_section_created_by; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue_section
    ADD CONSTRAINT fk_venue_section_created_by FOREIGN KEY (created_by) REFERENCES ticketing.sys_user(user_id);


--
-- Name: venue_section fk_venue_section_updated_by; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue_section
    ADD CONSTRAINT fk_venue_section_updated_by FOREIGN KEY (updated_by) REFERENCES ticketing.sys_user(user_id);


--
-- Name: venue fk_venue_updated_by; Type: FK CONSTRAINT; Schema: ticketing; Owner: -
--

ALTER TABLE ONLY ticketing.venue
    ADD CONSTRAINT fk_venue_updated_by FOREIGN KEY (updated_by) REFERENCES ticketing.sys_user(user_id);


--
-- PostgreSQL database dump complete
--

\unrestrict DDNLY0z1PqID8ycWoF7l5yPYjdKsaeUlrODwJww3gLP29DmSYltU5LNtpRO1HZW

