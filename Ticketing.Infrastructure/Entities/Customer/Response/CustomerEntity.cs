using Ticketing.Infrastructure.Entities;

namespace Ticketing.Infrastructure.Entities.Customer.Response;

public class CustomerEntity : BaseEntity
{
    public long customer_id { get; set; }
    public string customer_code { get; set; } = string.Empty;
    public string username { get; set; } = string.Empty;
    public string? email { get; set; }
    public string? phone { get; set; }
    public string password_hash { get; set; } = string.Empty;
    public string full_name { get; set; } = string.Empty;
    public string? avatar_url { get; set; }
    public string status { get; set; } = string.Empty;
    public bool email_verified { get; set; }
    public DateTime? last_login_at { get; set; }
    public new DateTime created_at { get; set; }
    public new DateTime? updated_at { get; set; }
}
