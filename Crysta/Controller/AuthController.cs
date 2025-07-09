using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;
    private readonly PasswordHasher<AppUser> _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public AuthController(AnalyticPlatformContext context, IOptions<JwtSettings> jwtOptions)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<AppUser>();
        _jwtSettings = jwtOptions.Value;
    }

    // POST http://localhost:5146/api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        return BadRequest(ModelState);

        var user = await _context.AppUsers
            .Include(u => u.AppUserRoles)
            .ThenInclude(ur => ur.AppRole)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || user.DeletedAt != null)
            return Unauthorized("Invalid user or password.");

        var passwordVerification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (passwordVerification == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid user or password.");

        var roles = user.AppUserRoles.Select(ur => ur.AppRole.RoleName).ToList();
        var token = GenerateJwtToken(user, roles);

        return Ok(new AuthResponseDto
        {
            Token = token,
            FullName = user.FullName ?? "",
            Roles = roles
        });
    }

    private string GenerateJwtToken(AppUser user, List<string> roles)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var ttlMinutes = _jwtSettings.TokenTTLMinutes;
        var now = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(ttlMinutes),
            NotBefore = now.AddSeconds(-5),
            IssuedAt = now,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
