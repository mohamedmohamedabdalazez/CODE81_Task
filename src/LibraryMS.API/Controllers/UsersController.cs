using LibraryMS.Application.DTOs.Users;
using LibraryMS.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.API.Controllers;

[Authorize(Roles = "Administrator")]
public class UsersController : BaseApiController
{
    private readonly IUserService _service;
    public UsersController(IUserService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => ToResult(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => ToResult(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto) =>
        ToResult(await _service.CreateAsync(dto, CurrentUserId));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto) =>
        ToResult(await _service.UpdateAsync(id, dto, CurrentUserId));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) =>
        ToResult(await _service.DeleteAsync(id, CurrentUserId));
}
