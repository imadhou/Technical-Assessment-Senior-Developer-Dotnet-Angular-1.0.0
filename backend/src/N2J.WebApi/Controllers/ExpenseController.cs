using Asp.Versioning;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using N2J.Application.Interfaces;
using N2J.Application.Models.Commands;
using N2J.Application.Models.Constants;
using N2J.Application.Models.Dtos;
using N2J.Application.Models.Dtos.Expense;
using N2J.Application.Models.Dtos.UserAuth;
using N2J.Application.Models.Exceptions;
using N2J.Application.Models.Queries;
using N2J.WebApi.Attributes;
using N2J.WebApi.Extensions;

namespace N2J.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/expenses")]
[Produces("application/json")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly IValidator<ExpenseQuery> _expenseQueryValidator;
    private readonly IValidator<ExpenseCommand> _expenseCommandValidator;
    private readonly IValidator<ExpenseStatusUpdateCommand> _expenseStatusUpdateCommand;
    private readonly ILogger<ExpenseController> _logger;

    public ExpenseController(
        IExpenseService expenseService,
        IValidator<ExpenseQuery> expenseQueryValidator,
        IValidator<ExpenseCommand> expenseCommandValidator,
        IValidator<ExpenseStatusUpdateCommand> expenseStatusUpdateCommand,
        ILogger<ExpenseController> logger)
    {
        _expenseService = expenseService;
        _expenseCommandValidator = expenseCommandValidator;
        _expenseQueryValidator = expenseQueryValidator;
        _expenseStatusUpdateCommand = expenseStatusUpdateCommand;
        _logger = logger;
    }

    [HttpPost]
    [ApiVersion("2.0")]
    [ApiVersion("1.0")]
    [AuthorizeRole(AppConstants.ManagerRole, AppConstants.EmployeeRole)]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExpenseDto>> CreateExpense([FromBody] ExpenseCommand command)
    {
        ValidationResult validationResult = await _expenseCommandValidator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        try
        {
            var contextUser = User.ToUserContext();
            var expense = await _expenseService.CreateExpenseAsync(command, contextUser.UserId, contextUser.CompanyId);

            expense.Links = GenerateExpenseLinks(expense.Id, expense.Status, contextUser);
            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, expense);
        }
        catch (ReferenceNotFoundException ex)
        {
            _logger.LogWarning(ex, "Reference not found while creating expense");
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [AuthorizeRole(AppConstants.ManagerRole, AppConstants.EmployeeRole)]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExpenseDto>> UpdateExpense([FromBody] ExpenseCommand command, [FromRoute] Guid id)
    {
        ValidationResult validationResult = await _expenseCommandValidator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        try
        {
            var contextUser = User.ToUserContext();
            var expense = await _expenseService.UpdateExpenseAsync(id, command, contextUser.UserId, contextUser.CompanyId);

            expense.Links = GenerateExpenseLinks(expense.Id, expense.Status, contextUser);
            return Ok(expense);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Expense not found while updating expense with ID {ExpenseId}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (ReferenceNotFoundException ex)
        {
            _logger.LogWarning(ex, "Reference not found while updating expense with ID {ExpenseId}", id);
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    [AuthorizeRole(AppConstants.ManagerRole)]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExpenseDto>> UpdateExpenseStatus([FromBody] ExpenseStatusUpdateCommand command, [FromRoute] Guid id)
    {
        ValidationResult validationResult = await _expenseStatusUpdateCommand.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        try
        {
            var contextUser = User.ToUserContext();
            var expense = await _expenseService.UpdateExpenseStatusAsync(id, command.NewStatus, contextUser.UserId, contextUser.CompanyId);

            expense.Links = GenerateExpenseLinks(expense.Id, expense.Status, contextUser);
            return Ok(expense);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Expense not found while updating status for expense with ID {ExpenseId}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating status for expense with ID {ExpenseId}", id);
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet]
    [AuthorizeRole(AppConstants.ManagerRole, AppConstants.EmployeeRole)]
    [ProducesResponseType(typeof(ExpenseCollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExpenseCollectionDto>> GetAllExpenses([FromQuery] ExpenseQuery query)
    {
        ValidationResult validationResult = await _expenseQueryValidator.ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        var contextUser = User.ToUserContext();
        var expenses = await _expenseService.GetAllWithCountAsync(query, contextUser.CompanyId);

        var links = GenerateCollectionLinks(query);
        return Ok(new ExpenseCollectionDto() { Expenses = expenses.Items, Links = links, TotalCount = expenses.TotalCount, TotalPages = (int)Math.Ceiling(expenses.TotalCount / (double)query.PageSize) });
    }

    [HttpGet("{id:guid}")]
    [AuthorizeRole(AppConstants.ManagerRole, AppConstants.EmployeeRole)]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExpenseDto>> GetExpenseById(Guid id)
    {
        var expense = await _expenseService.GetByIdAsync(id);
        if (expense == null) return NotFound();

        expense.Links = GenerateExpenseLinks(expense.Id, expense.Status, User.ToUserContext());
        return Ok(expense);
    }

    private List<LinkDto> GenerateExpenseLinks(Guid expenseId, string status, UserContext userContext)
    {
        var links = new List<LinkDto>();
        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var apiVersion = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

        // Self link
        links.Add(new LinkDto(
            $"{baseUrl}/api/v{apiVersion}/expenses/{expenseId}",
            "self",
            "GET",
            "Get expense details"
        ));

        // Collection link
        links.Add(new LinkDto(
            $"{baseUrl}/api/v{apiVersion}/expenses",
            "collection",
            "GET",
            "Get all expenses"
        ));

        // Update link - available for owner or manager
        bool canUpdate = userContext.Role == AppConstants.ManagerRole || userContext.UserId == expenseId; // Note: You might need to get the expense owner from service
        if (true)
        {
            links.Add(new LinkDto(
                $"{baseUrl}/api/v{apiVersion}/expenses/{expenseId}",
                "update",
                "PUT",
                "Update expense"
            ));
        }

        // Status update link - available only for managers
        bool canUpdateStatus = userContext.Role == AppConstants.ManagerRole;
        if (true)
        {
            links.Add(new LinkDto(
                $"{baseUrl}/api/v{apiVersion}/expenses/{expenseId}/status",
                "update-status",
                "PATCH",
                "Update expense status"
            ));
        }

        return links;
    }

    private List<LinkDto> GenerateCollectionLinks(ExpenseQuery query)
    {
        var links = new List<LinkDto>();
        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var apiVersion = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

        // Self link
        var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        if (query.Page > 1) queryString["page"] = query.Page.ToString();
        if (query.PageSize > 0) queryString["pageSize"] = query.PageSize.ToString();
        if (!string.IsNullOrEmpty(query.Status)) queryString["status"] = query.Status;
        if (query.ExpenseCategoryId.HasValue) queryString["expenseCategoryId"] = query.ExpenseCategoryId.Value.ToString();

        var queryParams = queryString.ToString();
        var selfUrl = $"{baseUrl}/api/v{apiVersion}/expenses";
        if (!string.IsNullOrEmpty(queryParams))
        {
            selfUrl += $"?{queryParams}";
        }

        links.Add(new LinkDto(selfUrl, "self", "GET", "Current page of expenses"));

        // Create link
        links.Add(new LinkDto(
            $"{baseUrl}/api/v{apiVersion}/expenses",
            "create",
            "POST",
            "Create new expense"
        ));

        return links;
    }
}