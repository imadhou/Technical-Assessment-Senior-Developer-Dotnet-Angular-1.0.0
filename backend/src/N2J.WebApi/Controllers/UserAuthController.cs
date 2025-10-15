using Asp.Versioning;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using N2J.Application.Interfaces;
using N2J.Application.Models.Commands.UserAuth;
using N2J.Application.Models.Dtos.UserAuth;
using N2J.Application.Models.Exceptions;
using N2J.Application.Validators;
using N2J.WebApi.Extensions; // Assuming you have validators

namespace N2J.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json")]
public class UserAuthController : ControllerBase
{
    private readonly IUserAuthService _userAuthService;
    private readonly ILogger<UserAuthController> _logger;
    private readonly IValidator<LoginCommand> _loginCommandValidator;
    private readonly IValidator<RegisterCommand> _registerCommandValidator;

    public UserAuthController(
        IUserAuthService userAuthService,
        ILogger<UserAuthController> logger,
        IValidator<LoginCommand> loginCommandValidator,
        IValidator<RegisterCommand> registerCommandValidator)
    {
        _userAuthService = userAuthService;
        _logger = logger;
        _loginCommandValidator = loginCommandValidator;
        _registerCommandValidator = registerCommandValidator;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginCommand request)
    {
        ValidationResult validationResult = await _loginCommandValidator.ValidateAsync(request);
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
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var result = await _userAuthService.LoginAsync(request);

            _logger.LogInformation("User {Email} logged in successfully", request.Email);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            return Unauthorized(new
            {
                Message = ex.Message
            });
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterCommand request)
    {
        // Validation
        ValidationResult validationResult = await _registerCommandValidator.ValidateAsync(request);
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
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            var result = await _userAuthService.RegisterAsync(request);

            _logger.LogInformation("User {Email} registered successfully", request.Email);
            return Ok(result);
        }
        catch (UserAuthException ex)
        {
            _logger.LogWarning(ex, $"{ex.Message}");
            return BadRequest(new
            {
                Message = ex.Message
            });
        }
    }


    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        try
        {
            // You'll need to implement this method in your service
            var userProfile = await _userAuthService.GetUserByIdAsync(User.ToUserContext().UserId);

            if (userProfile == null)
            {
                return NotFound(new
                {
                    Message = "User profile not found"
                });
            }

            return Ok(userProfile);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning("Unauthorized access attempt to GetCurrentUser");
            return Unauthorized(new
            {
                Message = ex.Message
            });
        }
    }
}