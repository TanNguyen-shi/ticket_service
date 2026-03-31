namespace Ticketing.Domain.Constants;

/// <summary>
/// Domain Service message constants for all operations
/// </summary>
public static class DomainMessageConstants
{
    #region Event
    public static class Event
    {
        public const string InsertSuccess = "Thêm mới sự kiện thành công";
        public const string InsertError = "Thêm mới sự kiện thất bại";
        public const string UpdateSuccess = "Cập nhật sự kiện thành công";
        public const string UpdateError = "Cập nhật sự kiện thất bại";
        public const string DeleteSuccess = "Xóa sự kiện thành công";
        public const string DeleteError = "Xóa sự kiện thất bại";
        public const string NotFound = "Không tìm thấy thông tin sự kiện";
        public const string GetDetailSuccess = "Lấy chi tiết sự kiện thành công";
        public const string GetListSuccess = "Lấy danh sách sự kiện thành công";
    }
    #endregion

    #region EventZone
    public static class EventZone
    {
        public const string InsertSuccess = "Thêm mới vùng sự kiện thành công";
        public const string InsertError = "Thêm mới vùng sự kiện thất bại";
        public const string UpdateSuccess = "Cập nhật vùng sự kiện thành công";
        public const string UpdateError = "Cập nhật vùng sự kiện thất bại";
        public const string DeleteSuccess = "Xóa vùng sự kiện thành công";
        public const string DeleteError = "Xóa vùng sự kiện thất bại";
        public const string NotFound = "Không tìm thấy thông tin vùng sự kiện";
        public const string GetDetailSuccess = "Lấy chi tiết vùng sự kiện thành công";
        public const string GetListSuccess = "Lấy danh sách vùng sự kiện thành công";
    }
    #endregion

    #region EventZonePrice
    public static class EventZonePrice
    {
        public const string InsertSuccess = "Thêm mới giá vé vùng sự kiện thành công";
        public const string InsertError = "Thêm mới giá vé vùng sự kiện thất bại";
        public const string UpdateSuccess = "Cập nhật giá vé vùng sự kiện thành công";
        public const string UpdateError = "Cập nhật giá vé vùng sự kiện thất bại";
        public const string DeleteSuccess = "Xóa giá vé vùng sự kiện thành công";
        public const string DeleteError = "Xóa giá vé vùng sự kiện thất bại";
        public const string NotFound = "Không tìm thấy thông tin giá vé vùng sự kiện";
        public const string GetDetailSuccess = "Lấy chi tiết giá vé vùng sự kiện thành công";
        public const string GetListSuccess = "Lấy danh sách giá vé vùng sự kiện thành công";
    }
    #endregion

    #region EventSeatInventory
    public static class EventSeatInventory
    {
        public const string InsertSuccess = "Thêm mới kho hàng ghế sự kiện thành công";
        public const string InsertError = "Thêm mới kho hàng ghế sự kiện thất bại";
        public const string UpdateSuccess = "Cập nhật kho hàng ghế sự kiện thành công";
        public const string UpdateError = "Cập nhật kho hàng ghế sự kiện thất bại";
        public const string DeleteSuccess = "Xóa kho hàng ghế sự kiện thành công";
        public const string DeleteError = "Xóa kho hàng ghế sự kiện thất bại";
        public const string NotFound = "Không tìm thấy thông tin kho hàng ghế sự kiện";
        public const string GetDetailSuccess = "Lấy chi tiết kho hàng ghế sự kiện thành công";
        public const string GetListSuccess = "Lấy danh sách kho hàng ghế sự kiện thành công";
    }
    #endregion

    #region EventPublishLog
    public static class EventPublishLog
    {
        public const string InsertSuccess = "Thêm mới nhật ký xuất bản sự kiện thành công";
        public const string InsertError = "Thêm mới nhật ký xuất bản sự kiện thất bại";
        public const string UpdateSuccess = "Cập nhật nhật ký xuất bản sự kiện thành công";
        public const string UpdateError = "Cập nhật nhật ký xuất bản sự kiện thất bại";
        public const string DeleteSuccess = "Xóa nhật ký xuất bản sự kiện thành công";
        public const string DeleteError = "Xóa nhật ký xuất bản sự kiện thất bại";
        public const string NotFound = "Không tìm thấy thông tin nhật ký xuất bản sự kiện";
        public const string GetDetailSuccess = "Lấy chi tiết nhật ký xuất bản sự kiện thành công";
        public const string GetListSuccess = "Lấy danh sách nhật ký xuất bản sự kiện thành công";
    }
    #endregion

    #region Venue
    public static class Venue
    {
        public const string InsertSuccess = "Thêm mới địa điểm thành công";
        public const string InsertError = "Thêm mới địa điểm thất bại";
        public const string UpdateSuccess = "Cập nhật địa điểm thành công";
        public const string UpdateError = "Cập nhật địa điểm thất bại";
        public const string DeleteSuccess = "Xóa địa điểm thành công";
        public const string DeleteError = "Xóa địa điểm thất bại";
        public const string NotFound = "Không tìm thấy thông tin địa điểm";
        public const string GetDetailSuccess = "Lấy chi tiết địa điểm thành công";
        public const string GetListSuccess = "Lấy danh sách địa điểm thành công";
    }
    #endregion

    #region VenueSection
    public static class VenueSection
    {
        public const string InsertSuccess = "Thêm mới khu vực địa điểm thành công";
        public const string InsertError = "Thêm mới khu vực địa điểm thất bại";
        public const string UpdateSuccess = "Cập nhật khu vực địa điểm thành công";
        public const string UpdateError = "Cập nhật khu vực địa điểm thất bại";
        public const string DeleteSuccess = "Xóa khu vực địa điểm thành công";
        public const string DeleteError = "Xóa khu vực địa điểm thất bại";
        public const string NotFound = "Không tìm thấy thông tin khu vực địa điểm";
        public const string GetDetailSuccess = "Lấy chi tiết khu vực địa điểm thành công";
        public const string GetListSuccess = "Lấy danh sách khu vực địa điểm thành công";
    }
    #endregion

    #region VenueSeat
    public static class VenueSeat
    {
        public const string InsertSuccess = "Thêm mới ghế địa điểm thành công";
        public const string InsertError = "Thêm mới ghế địa điểm thất bại";
        public const string UpdateSuccess = "Cập nhật ghế địa điểm thành công";
        public const string UpdateError = "Cập nhật ghế địa điểm thất bại";
        public const string DeleteSuccess = "Xóa ghế địa điểm thành công";
        public const string DeleteError = "Xóa ghế địa điểm thất bại";
        public const string NotFound = "Không tìm thấy thông tin ghế địa điểm";
        public const string GetDetailSuccess = "Lấy chi tiết ghế địa điểm thành công";
        public const string GetListSuccess = "Lấy danh sách ghế địa điểm thành công";
    }
    #endregion

    #region Ticket
    public static class Ticket
    {
        public const string InsertSuccess = "Thêm mới vé thành công";
        public const string InsertError = "Thêm mới vé thất bại";
        public const string UpdateSuccess = "Cập nhật vé thành công";
        public const string UpdateError = "Cập nhật vé thất bại";
        public const string DeleteSuccess = "Xóa vé thành công";
        public const string DeleteError = "Xóa vé thất bại";
        public const string NotFound = "Không tìm thấy thông tin vé";
        public const string GetDetailSuccess = "Lấy chi tiết vé thành công";
        public const string GetListSuccess = "Lấy danh sách vé thành công";
    }
    #endregion

    #region TicketOrder
    public static class TicketOrder
    {
        public const string InsertSuccess = "Thêm mới đơn hàng vé thành công";
        public const string InsertError = "Thêm mới đơn hàng vé thất bại";
        public const string UpdateSuccess = "Cập nhật đơn hàng vé thành công";
        public const string UpdateError = "Cập nhật đơn hàng vé thất bại";
        public const string DeleteSuccess = "Xóa đơn hàng vé thành công";
        public const string DeleteError = "Xóa đơn hàng vé thất bại";
        public const string NotFound = "Không tìm thấy thông tin đơn hàng vé";
        public const string GetDetailSuccess = "Lấy chi tiết đơn hàng vé thành công";
        public const string GetListSuccess = "Lấy danh sách đơn hàng vé thành công";
    }
    #endregion

    #region TicketOrderItem
    public static class TicketOrderItem
    {
        public const string InsertSuccess = "Thêm mới chi tiết đơn hàng vé thành công";
        public const string InsertError = "Thêm mới chi tiết đơn hàng vé thất bại";
        public const string UpdateSuccess = "Cập nhật chi tiết đơn hàng vé thành công";
        public const string UpdateError = "Cập nhật chi tiết đơn hàng vé thất bại";
        public const string DeleteSuccess = "Xóa chi tiết đơn hàng vé thành công";
        public const string DeleteError = "Xóa chi tiết đơn hàng vé thất bại";
        public const string NotFound = "Không tìm thấy thông tin chi tiết đơn hàng vé";
        public const string GetDetailSuccess = "Lấy chi tiết chi tiết đơn hàng vé thành công";
        public const string GetListSuccess = "Lấy danh sách chi tiết đơn hàng vé thành công";
    }
    #endregion

    #region SysRole
    public static class SysRole
    {
        public const string InsertSuccess = "Thêm mới vai trò thành công";
        public const string InsertError = "Thêm mới vai trò thất bại";
        public const string UpdateSuccess = "Cập nhật vai trò thành công";
        public const string UpdateError = "Cập nhật vai trò thất bại";
        public const string DeleteSuccess = "Xóa vai trò thành công";
        public const string DeleteError = "Xóa vai trò thất bại";
        public const string NotFound = "Không tìm thấy thông tin vai trò";
        public const string GetDetailSuccess = "Lấy chi tiết vai trò thành công";
        public const string GetListSuccess = "Lấy danh sách vai trò thành công";
    }
    #endregion

    #region SysUser
    public static class SysUser
    {
        public const string InsertSuccess = "Thêm mới người dùng thành công";
        public const string InsertError = "Thêm mới người dùng thất bại";
        public const string UpdateSuccess = "Cập nhật người dùng thành công";
        public const string UpdateError = "Cập nhật người dùng thất bại";
        public const string DeleteSuccess = "Xóa người dùng thành công";
        public const string DeleteError = "Xóa người dùng thất bại";
        public const string NotFound = "Không tìm thấy thông tin người dùng";
        public const string GetDetailSuccess = "Lấy chi tiết người dùng thành công";
        public const string GetListSuccess = "Lấy danh sách người dùng thành công";
    }
    #endregion

    #region SysUserRole
    public static class SysUserRole
    {
        public const string InsertSuccess = "Thêm mới quyền người dùng thành công";
        public const string InsertError = "Thêm mới quyền người dùng thất bại";
        public const string UpdateSuccess = "Cập nhật quyền người dùng thành công";
        public const string UpdateError = "Cập nhật quyền người dùng thất bại";
        public const string DeleteSuccess = "Xóa quyền người dùng thành công";
        public const string DeleteError = "Xóa quyền người dùng thất bại";
        public const string NotFound = "Không tìm thấy thông tin quyền người dùng";
        public const string GetDetailSuccess = "Lấy chi tiết quyền người dùng thành công";
        public const string GetListSuccess = "Lấy danh sách quyền người dùng thành công";
    }
    #endregion
}

