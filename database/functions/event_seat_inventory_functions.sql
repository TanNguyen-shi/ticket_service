-- =========================================================
-- MODULE: EVENT_SEAT_INVENTORY
-- TABLE : ticketing.event_seat_inventory
-- NOTE  : refcursor, read-only (write ops are in checkout_functions.sql)
-- =========================================================

-- DROP FUNCTION IF EXISTS ticketing.event_seat_inventory_getbyid(bigint);
-- DROP FUNCTION IF EXISTS ticketing.event_seat_inventory_getpagedlist(integer, integer, varchar, bigint, bigint, varchar);



CREATE OR REPLACE FUNCTION ticketing.event_seat_inventory_getbyid(
    p_event_seat_inventory_id bigint
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
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
$function$;



CREATE OR REPLACE FUNCTION ticketing.event_seat_inventory_getpagedlist(
    p_pagesize integer,
    p_offset integer,
    p_keysearch varchar,
    p_event_id bigint,
    p_event_zone_id bigint,
    p_seat_status varchar
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
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
$function$;
