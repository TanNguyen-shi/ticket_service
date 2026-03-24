using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace TicketingSystem.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseApiController() : ControllerBase
    {
    }
}