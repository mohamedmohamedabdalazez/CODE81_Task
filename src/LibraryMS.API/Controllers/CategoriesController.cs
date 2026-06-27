using LibraryMS.Application.DTOs.Categories;
using LibraryMS.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.API.Controllers;

public class CategoriesController : BaseApiController
{
    private readonly ICategoryService _service;
    public CategoriesController(ICategoryService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => ToResult(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => ToResult(await _service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto) =>
        ToResult(await _service.CreateAsync(dto, CurrentUserId));

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto) =>
        ToResult(await _service.UpdateAsync(id, dto, CurrentUserId));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id) =>
        ToResult(await _service.DeleteAsync(id, CurrentUserId));
}
