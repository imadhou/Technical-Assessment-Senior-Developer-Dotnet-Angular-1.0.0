using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using N2J.Application.Interfaces;
using N2J.Application.Models.Commands.UserAuth;
using N2J.Application.Models.Constants;
using N2J.Application.Models.Dtos.UserAuth;
using N2J.Domain.Entities;
using N2J.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace N2J.Application.Services;

public class UserAuthService : IUserAuthService
{

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly JwtSettings _jwtSettings;


    public UserAuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginCommand request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Verify password
        if (!await VerifyPasswordAsync(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = GenerateJwtToken(user);
        return token;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterCommand request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Get default role (you might want to make this configurable)
        var defaultRole = await _roleRepository.GetByNameAsync(AppConstants.EmployeeRole);
        if (defaultRole == null)
        {
            throw new InvalidOperationException("Default role not found");
        }

        string passwordHash = await HashPasswordAsync(request.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CompanyId = request.CompanyId ?? AppConstants.DefaultCompanyId,
            RoleId = defaultRole.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, logChanges: true);

        var token = GenerateJwtToken(user);
        return token;
    }


    private AuthResponseDto GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var roleName = user.Role?.Name ?? "User";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, roleName),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim("companyId", user.CompanyId.ToString()),
            new Claim("roleId", user.RoleId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return new AuthResponseDto
        {
            Token = tokenString,
            Expires = token.ValidTo,
            UserId = user.Id.ToString(),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = roleName,
        };
    }

    public async Task<string> HashPasswordAsync(string password)
    {
        return await Task.Run(() =>
        {
            // Generate a 128-bit salt using a cryptographically strong random sequence
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            // Combine salt and hash
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        });
    }

    public async Task<bool> VerifyPasswordAsync(string password, string storedHash)
    {
        return await Task.Run(() =>
        {
            try
            {
                var parts = storedHash.Split('.', 2);
                if (parts.Length != 2)
                {
                    return false;
                }

                var salt = Convert.FromBase64String(parts[0]);
                var storedSubkey = parts[1];

                var computedSubkey = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                return storedSubkey == computedSubkey;
            }
            catch
            {
                return false;
            }
        });
    }
}