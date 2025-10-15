using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using N2J.Application.Interfaces;
using N2J.Application.Models.Commands.UserAuth;
using N2J.Application.Models.Dtos.UserAuth;

namespace N2J.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class UserAuthController : ControllerBase
{
    private readonly IUserAuthService _userAuthService;
    private readonly ILogger<UserAuthController> _logger;

    public UserAuthController(
        IUserAuthService userAuthService,
        ILogger<UserAuthController> logger)
    {
        _userAuthService = userAuthService;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
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
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return BadRequest(new ProblemDetails
            {
                Title = "Login failed",
                Detail = "An error occurred during login",
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        try
        {
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);
            
            // Basic validation
            if (request.Password.Length < 6)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Validation failed",
                    Detail = "Password must be at least 6 characters long",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var result = await _userAuthService.RegisterAsync(request);
            
            _logger.LogInformation("User {Email} registered successfully", request.Email);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed for email: {Email} - {Error}", request.Email, ex.Message);
            return BadRequest(new ProblemDetails
            {
                Title = "Registration failed",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return BadRequest(new ProblemDetails
            {
                Title = "Registration failed",
                Detail = "An error occurred during registration",
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /*[HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var firstName = User.FindFirst("firstName")?.Value;
            var lastName = User.FindFirst("lastName")?.Value;
            var companyId = User.FindFirst("companyId")?.Value;
            var roleId = User.FindFirst("roleId")?.Value;

            var userProfile = new UserProfileDto
            {
                UserId = userId,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Role = role,
                CompanyId = companyId != null ? Guid.Parse(companyId) : Guid.Empty,
                RoleId = roleId != null ? Guid.Parse(roleId) : Guid.Empty
            };

            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user profile");
            return Unauthorized();
        }
    }*/

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult RefreshToken()
    {
        // This endpoint would typically validate the current token and issue a new one
        // For now, we'll return 401 - the client should login again
        return Unauthorized(new ProblemDetails
        {
            Title = "Token refresh not implemented",
            Detail = "Please login again to get a new token",
            Status = StatusCodes.Status401Unauthorized
        });
    }
}