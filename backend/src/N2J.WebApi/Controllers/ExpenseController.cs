using Asp.Versioning;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N2J.Application.Interfaces;
using N2J.Application.Models.Commands;
using N2J.Application.Models.Constants;
using N2J.Application.Models.Queries;
using N2J.Application.Validators;
using N2J.WebApi.Attributes;
using N2J.WebApi.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace N2J.WebApi.Controllers;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/expenses")]
public class ExpenseController: ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly IValidator<ExpenseQuery> _expenseQueryValidator;
    private readonly IValidator<ExpenseCommand> _expenseCommandValidator;
    private readonly IValidator<ExpenseStatusUpdateCommand> _expenseStatusUpdateCommand;
    public ExpenseController(IExpenseService expenseService, IValidator<ExpenseQuery> expenseQueryValidator, IValidator<ExpenseCommand> expenseCommandValidator, IValidator<ExpenseStatusUpdateCommand> expenseStatusUpdateCommand)
    {
        _expenseService = expenseService;
        _expenseCommandValidator = expenseCommandValidator;
        _expenseQueryValidator = expenseQueryValidator;
        _expenseStatusUpdateCommand = expenseStatusUpdateCommand;
    }


    [HttpPost]
    [AuthorizeRole(AppConstants.EmployeeRole)]

    public async Task<IActionResult> CreateExpense([FromBody] ExpenseCommand command)
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
        var expenses = await _expenseService.CreateExpense(command, User.ToUserContext().UserId);
        return Ok(expenses);
    }

    [HttpPut("{id:guid}")]
    [AuthorizeRole(AppConstants.EmployeeRole)]

    public async Task<IActionResult> UpdateExpense([FromBody] ExpenseCommand command, [FromRoute] Guid id)
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
        var expenses = await _expenseService.UpdateExpense(id, command, User.ToUserContext().UserId);
        return Ok(expenses);
    }


    [HttpPatch("{id:guid}/status")]
    [AuthorizeRole(AppConstants.ManagerRole)]

    public async Task<IActionResult> UpdateExpenseStatus([FromBody] ExpenseStatusUpdateCommand command, [FromRoute] Guid id)
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
        var expenses = await _expenseService.UpdateExpenseStatus(id, command.NewStatus, User.ToUserContext().UserId);
        return Ok(expenses);
    }

    [HttpGet]
    [AuthorizeRole(AppConstants.EmployeeRole)]
    public async Task<IActionResult> GetAllExpenses([FromQuery] ExpenseQuery query)
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
        var expenses = await _expenseService.GetAll(query);
        return Ok(expenses);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetExpenseById(Guid id)
    {
        var expense = await _expenseService.GetById(id);
        if (expense == null) return NotFound();
        return Ok(expense);
    }

}

