using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.SysUser.Request;

public class SysUserGetByUserRequest
{
    public string username { get; set; }
}