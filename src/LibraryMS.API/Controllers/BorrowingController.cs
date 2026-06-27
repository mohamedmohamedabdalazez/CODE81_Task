using LibraryMS.Application.DTOs.Borrowing;
using LibraryMS.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.API.Controllers;

public class BorrowingController : BaseApiController
{
    private readonly IBorrowingService _service;
    public BorrowingController(IBorrowingService service) => _service = service;

    [HttpPost("borrow")]
    public async Task<IActionResult> Borrow([FromBody] BorrowBookDto dto) =>
        ToResult(await _service.BorrowAsync(dto, CurrentUserId));

    [HttpPost("return/{transactionId}")]
    public async Task<IActionResult> Return(int transactionId) =>
        ToResult(await _service.ReturnAsync(transactionId, CurrentUserId));

    [HttpGet("transactions")]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> GetAll() => ToResult(await _service.GetAllTransactionsAsync());

    [HttpGet("member/{memberId}")]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> GetByMember(int memberId) =>
        ToResult(await _service.GetByMemberAsync(memberId));
}
