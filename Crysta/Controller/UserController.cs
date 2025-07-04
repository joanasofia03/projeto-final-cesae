using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;
    private readonly PasswordHasher<AppUser> _passwordHasher;

    public UsersController(AnalyticPlatformContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<AppUser>();
    }

    // http://localhost:5146/api/users
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateAppUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.AppUsers.AnyAsync(u => u.Email == dto.Email))
            return Conflict("This email is already registered.");

        if (await _context.AppUsers.AnyAsync(u => u.DocumentId == dto.DocumentId))
            return Conflict("This document is already registered.");

        var clientRole = await _context.AppRoles.FirstOrDefaultAsync(r => r.RoleName == "Client");
        if (clientRole == null)
            return StatusCode(500, "Client role not found in the database.");

        var user = new AppUser
        {
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            DocumentId = dto.DocumentId,
            BirthDate = dto.BirthDate,
            CreationDate = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        var userRole = new AppUserRole
        {
            AppUserId = user.Id,
            AppRoleId = clientRole.Id
        };

        _context.AppUserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.PhoneNumber,
            user.DocumentId,
            user.BirthDate
        });
    }

    // http://localhost:5146/api/users/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _context.AppUsers.FindAsync(id);

        if (user == null || user.DeletedAt != null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.PhoneNumber,
            user.DocumentId,
            user.BirthDate
        });
    }
}