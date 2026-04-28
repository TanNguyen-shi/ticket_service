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
