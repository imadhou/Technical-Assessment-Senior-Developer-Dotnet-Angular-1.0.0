// WebApi/Extensions/ClaimsPrincipalExtensions.cs
using N2J.Application.Models.Dtos.UserAuth;
using System.Security.Claims;

namespace N2J.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID claim not found");
    }

    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Email)
            ?? throw new InvalidOperationException("Email claim not found");
    }

    public static string GetFirstName(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("firstName")
            ?? throw new InvalidOperationException("First name claim not found");
    }

    public static string GetLastName(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("lastName")
            ?? throw new InvalidOperationException("Last name claim not found");
    }

    public static Guid GetCompanyId(this ClaimsPrincipal principal)
    {
        var companyId = principal.FindFirstValue("companyId");
        if (string.IsNullOrEmpty(companyId) || !Guid.TryParse(companyId, out var result))
        {
            throw new InvalidOperationException("Company ID claim not found or invalid");
        }
        return result;
    }

    public static string GetRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Role)
            ?? throw new InvalidOperationException("Role claim not found");
    }

    public static bool IsInRole(this ClaimsPrincipal principal, string role)
    {
        return principal.IsInRole(role);
    }

    public static UserContext ToUserContext(this ClaimsPrincipal principal)
    {
        return new UserContext
        {
            UserId = Guid.Parse(principal.GetUserId()),
            Email = principal.GetEmail(),
            FirstName = principal.GetFirstName(),
            LastName = principal.GetLastName(),
            Role = principal.GetRole(),
            CompanyId = principal.GetCompanyId(),
            RoleId = Guid.TryParse(principal.FindFirstValue("roleId"), out var roleId) ? roleId : Guid.Empty
        };
    }
}