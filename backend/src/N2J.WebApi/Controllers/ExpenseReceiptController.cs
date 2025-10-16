using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N2J.Application.Interfaces;
using N2J.Application.Models.Commands;
using N2J.Application.Models.Dtos.Expense;
using N2J.WebApi.Controllers;
using N2J.WebApi.Extensions;

namespace N2J.Application.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/expenses-receipt")]
[Produces("application/json")]
public class ExpenseReceiptsController : ControllerBase
{
    private readonly IExpenseReceiptService _receiptService;
    private readonly ILogger<ExpenseReceiptsController> _logger;

    public ExpenseReceiptsController(IExpenseReceiptService receiptService, ILogger<ExpenseReceiptsController> logger)
    {
        _receiptService = receiptService;
        _logger = logger;
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
            _logger.LogWarning(ex, "Unauthorized access while uploading receipt for expense {ExpenseId}", request.ExpenseId);
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid file upload attempt for expense {ExpenseId}", request.ExpenseId);
            return BadRequest(new { message = ex.Message });
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
            _logger.LogWarning("Receipt not found for expense {ExpenseId}", expenseId);
            return NotFound(new { message = "Receipt not found" });
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Unauthorized access while downloading receipt for expense {ExpenseId}", expenseId);
            return NotFound(new { message = "Receipt not found" });
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
            _logger.LogWarning("Unauthorized access while deleting receipt for expense {ExpenseId}", expenseId);
            return NotFound();
        }
    }
}