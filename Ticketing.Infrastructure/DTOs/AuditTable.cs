namespace Ticketing.Infrastructure.DTOs;

public class AuditTable
{
    public long? created_by { get; set; }
    public DateTime created_at { get; set; }
    public string created_fullname { get; set; }
    public long? updated_by { get; set; }
    public DateTime? updated_at { get; set; }
    public string updated_fullname { get; set; }
    public long? deleted_by { get; set; }
    public DateTime? deleted_at { get; set; }
    public string deleted_fullname { get; set; }
}