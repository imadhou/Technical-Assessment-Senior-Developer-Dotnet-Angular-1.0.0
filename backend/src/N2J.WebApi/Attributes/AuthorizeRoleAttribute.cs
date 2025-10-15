using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace N2J.WebApi.Attributes;

public class AuthorizeRoleAttribute : ActionFilterAttribute
{
    private readonly string[] _roles;

    public AuthorizeRoleAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Check if user is authenticated
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Message = "Authentication required",
                StatusCode = 401
            });
            return;
        }

        // Get user roles from claims
        var userRoles = context.HttpContext.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Check if user has any of the required roles
        if (!_roles.Any(requiredRole => userRoles.Contains(requiredRole)))
        {
            context.Result = new ForbidResult();
            return;
        }

        base.OnActionExecuting(context);
    }
}