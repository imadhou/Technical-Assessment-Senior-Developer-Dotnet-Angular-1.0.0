using Asp.Versioning;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using N2J.Application.Interfaces;
using N2J.Application.Models.Queries;

namespace N2J.WebApi.Controllers;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/expenses")]
public class ExpenseController: ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly IValidator<ExpenseQuery> _expenseValidator;
    public ExpenseController(IExpenseService expenseService, IValidator<ExpenseQuery> expenseValidator)
    {
        _expenseService = expenseService;
        _expenseValidator = expenseValidator;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ExpenseQuery query)
    {
        ValidationResult validationResult = await _expenseValidator.ValidateAsync(query);

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
    public async Task<IActionResult> GetById(Guid id)
    {
        var expense = await _expenseService.GetById(id);
        if (expense == null) return NotFound();
        return Ok(expense);
    }

}

