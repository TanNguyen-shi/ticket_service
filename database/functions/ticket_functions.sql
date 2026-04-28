-- ============================================================
-- Ticket query functions
-- ============================================================

-- DROP FUNCTION IF EXISTS ticketing.ticket_getbycustomerid(bigint);

CREATE OR REPLACE FUNCTION ticketing.ticket_getbycustomerid(
    p_customer_id BIGINT
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.ticket_getbycustomerid(1);
    FETCH ALL IN "ticket_getbycustomerid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_getbycustomerid';
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
        o.paid_at,
        t.created_at
    FROM ticketing.ticket t
    INNER JOIN ticketing.ticket_order_item oi ON oi.ticket_order_item_id = t.order_item_id
    INNER JOIN ticketing.ticket_order o ON o.order_id = oi.order_id
    WHERE t.customer_id = p_customer_id
      AND t.is_deleted   = FALSE
    ORDER BY t.issued_at DESC;

    RETURN v_out;
END;
$function$;


-- DROP FUNCTION IF EXISTS ticketing.ticket_getbyid(bigint, bigint);

CREATE OR REPLACE FUNCTION ticketing.ticket_getbyid(
    p_ticket_id   BIGINT,
    p_customer_id BIGINT
)
RETURNS refcursor
LANGUAGE plpgsql
AS $function$
/*
    BEGIN;
    SELECT ticketing.ticket_getbyid(1, 1);
    FETCH ALL IN "ticket_getbyid";
    COMMIT;
*/
DECLARE
    v_out refcursor := 'ticket_getbyid';
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
        oi.unit_price,
        t.created_at
    FROM ticketing.ticket t
    INNER JOIN ticketing.ticket_order_item oi ON oi.ticket_order_item_id = t.order_item_id
    INNER JOIN ticketing.ticket_order o ON o.order_id = oi.order_id
    WHERE t.ticket_id   = p_ticket_id
      AND t.customer_id = p_customer_id
      AND t.is_deleted  = FALSE;

    RETURN v_out;
END;
$function$;
