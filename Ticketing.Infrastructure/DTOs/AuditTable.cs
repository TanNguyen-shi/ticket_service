namespace Ticketing.Infrastructure.DTOs;

public class AuditTable
{
    public long? created_by { get; set; }
    public DateTime created_at { get; set; }
    public string created_by_name { get; set; }
    public long? updated_by { get; set; }
    public DateTime? updated_at { get; set; }
    public string updated_by_name { get; set; }
}