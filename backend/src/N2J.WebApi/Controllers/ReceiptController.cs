using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N2J.Application.Interfaces;
using N2J.Application.Models.Commands;
using N2J.Application.Models.Dtos.Expense;
using N2J.WebApi.Extensions;

namespace N2J.Application.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/expenses-receipt")]
[Produces("application/json")]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _receiptService;

    public ReceiptsController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReceiptDto>> CreateReceipt([FromForm] FileUploadCommand request)
    {
        try
        {
            var userId = User.ToUserContext().UserId;
            var result = await _receiptService.UploadReceiptAsync(request, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while uploading the receipt." });
        }
    }

    [HttpGet("{expenseId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReceipt(Guid expenseId)
    {
        try
        {
            var userId = User.ToUserContext().UserId;
            var stream = await _receiptService.DownloadReceiptAsync(expenseId, userId);

            return File(stream, "application/octet-stream", $"receipt-{expenseId}");
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { message = "Receipt not found" });
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound(new { message = "Receipt not found" }); // Don't reveal existence
        }
    }

    [HttpDelete("{expenseId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReceipt(Guid expenseId)
    {
        try
        {
            var userId = User.ToUserContext().UserId;
            var result = await _receiptService.DeleteReceiptAsync(expenseId, userId);

            return result ? NoContent() : NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound();
        }
    }
}