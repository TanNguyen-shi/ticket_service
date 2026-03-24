namespace Ticketing.Infrastructure.Entities;

public class BaseEntity
{
    public DateTime created_at { get; set; }
    public long? created_by { get; set; }
    public long? updated_by { get; set; }
    public long? deleted_by { get; set; }
    public DateTime? updated_at { get; set; }
    public bool is_deleted { get; set; }
}