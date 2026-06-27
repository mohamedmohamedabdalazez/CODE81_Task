using LibraryMS.Application.DTOs.Members;
using LibraryMS.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.API.Controllers;

[Authorize(Roles = "Administrator,Librarian")]
public class MembersController : BaseApiController
{
    private readonly IMemberService _service;
    public MembersController(IMemberService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => ToResult(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => ToResult(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMemberDto dto) =>
        ToResult(await _service.CreateAsync(dto, CurrentUserId));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMemberDto dto) =>
        ToResult(await _service.UpdateAsync(id, dto, CurrentUserId));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id) =>
        ToResult(await _service.DeleteAsync(id, CurrentUserId));
}
