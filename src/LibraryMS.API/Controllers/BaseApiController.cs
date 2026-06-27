using System.Security.Claims;
using LibraryMS.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    protected IActionResult ToResult<T>(ServiceResult<T> result)
    {
        var response = ApiResponse<T>.From(result);
        return StatusCode(result.StatusCode, response);
    }
}
