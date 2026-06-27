using LibraryMS.Application.DTOs.Books;
using LibraryMS.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.API.Controllers;

public class BooksController : BaseApiController
{
    private readonly IBookService _service;
    public BooksController(IBookService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => ToResult(await _service.GetAllAsync());

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] string? author, [FromQuery] string? category) =>
        ToResult(await _service.SearchAsync(name, author, category));

    [HttpGet("by-status")]
    public async Task<IActionResult> ByStatus([FromQuery] string status) =>
        ToResult(await _service.GetByStatusAsync(status));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => ToResult(await _service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> Create([FromBody] CreateBookDto dto) =>
        ToResult(await _service.CreateAsync(dto, CurrentUserId));

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto) =>
        ToResult(await _service.UpdateAsync(id, dto, CurrentUserId));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id) =>
        ToResult(await _service.DeleteAsync(id, CurrentUserId));

    [HttpPost("{id}/cover")]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> UploadCover(int id, IFormFile file) =>
        ToResult(await _service.UploadCoverAsync(id, file, CurrentUserId));
}
