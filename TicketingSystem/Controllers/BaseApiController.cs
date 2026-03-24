using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;

namespace TicketingSystem.Controllers
{
    [Authorize]
    [ApiController]
    [ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class BaseApiController() : ControllerBase
    {
    }
}