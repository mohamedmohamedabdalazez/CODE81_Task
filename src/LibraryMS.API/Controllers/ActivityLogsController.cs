using LibraryMS.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.API.Controllers;

[Authorize(Roles = "Administrator,Librarian")]
public class ActivityLogsController : BaseApiController
{
    private readonly IActivityLogService _service;
    public ActivityLogsController(IActivityLogService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => ToResult(await _service.GetAllAsync());

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetByUser(int userId) =>
        ToResult(await _service.GetByUserAsync(userId));
}
