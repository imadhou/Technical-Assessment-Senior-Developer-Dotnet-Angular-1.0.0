using N2J.Application.Models.Commands.UserAuth;
using N2J.Application.Models.Dtos.UserAuth;

namespace N2J.Application.Interfaces;

public interface IUserAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterCommand command);
    Task<AuthResponseDto> LoginAsync(LoginCommand command);
    Task<string> HashPasswordAsync(string password);
    Task<bool> VerifyPasswordAsync(string password, string passwordHash);
}