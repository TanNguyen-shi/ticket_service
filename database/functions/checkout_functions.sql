-- ============================================================
-- Checkout helper functions
-- ============================================================

-- DROP FUNCTION IF EXISTS ticketing.seat_hold_item_updatestatusbyholdid(bigint, varchar);

CREATE OR REPLACE FUNCTION ticketing.seat_hold_item_updatestatusbyholdid(
    p_hold_id   BIGINT,
    p_status    VARCHAR(50)
)
RETURNS refcursor AS
$$
DECLARE
    v_out refcursor := 'seat_hold_item_updatestatusbyholdid';
BEGIN
    UPDATE ticketing.seat_hold_item
    SET    status     = p_status,
           updated_at = NOW()
    WHERE  hold_id    = p_hold_id
      AND  is_deleted = FALSE;

    OPEN v_out FOR
        SELECT p_hold_id AS hold_id;

    RETURN v_out;
END;
$$
LANGUAGE plpgsql;


-- ============================================================
-- Release: reset event_seat_inventory từ "held" → "available"
-- Chỉ release nếu ghế đang ở trạng thái "held" (guard chống double-release)
-- ============================================================

-- DROP FUNCTION IF EXISTS ticketing.event_seat_inventory_update_release(bigint, bigint, bigint, bigint);

CREATE OR REPLACE FUNCTION ticketing.event_seat_inventory_update_release(
    p_event_seat_inventory_id  bigint,
    p_event_id                 bigint,
    p_seat_id                  bigint,
    p_event_zone_id            bigint
)
RETURNS refcursor AS
$$
DECLARE
    v_out refcursor := 'event_seat_inventory_update_release';
BEGIN
    UPDATE ticketing.event_seat_inventory
    SET
        seat_status     = 'available',
        current_hold_id = NULL,
        version         = version + 1,
        updated_at      = now()
    WHERE event_seat_inventory_id = p_event_seat_inventory_id
      AND event_id                = p_event_id
      AND seat_id                 = p_seat_id
      AND seat_status             = 'held';   -- guard: chỉ nhả nếu đang bị held

    OPEN v_out FOR
        SELECT p_event_seat_inventory_id AS event_seat_inventory_id;

    RETURN v_out;
END;
$$
LANGUAGE plpgsql;


-- ============================================================
-- Lấy danh sách hold_id của các phiên giữ chỗ đã hết hạn
-- (status = 'active' AND hold_expires_at < now())
-- Dùng cho background job tự động nhả ghế
-- ============================================================

-- DROP FUNCTION IF EXISTS ticketing.seat_hold_getexpiredactive();

CREATE OR REPLACE FUNCTION ticketing.seat_hold_getexpiredactive()
RETURNS refcursor AS
$$
DECLARE
    v_out refcursor := 'seat_hold_getexpiredactive';
BEGIN
    OPEN v_out FOR
        SELECT hold_id
        FROM   ticketing.seat_hold
        WHERE  status          = 'active'
          AND  hold_expires_at < now();

    RETURN v_out;
END;
$$
LANGUAGE plpgsql;
