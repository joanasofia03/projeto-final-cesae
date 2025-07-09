using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class AdminController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;
    private readonly PasswordHasher<AppUser> _passwordHasher;
    public AdminController(AnalyticPlatformContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<AppUser>();
    }

    // POST http://localhost:5146/api/admin/create-admin
    [Authorize(Roles = "Administrator")]
    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAppUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.AppUsers.AnyAsync(u => u.Email == dto.Email))
            return Conflict("This email is already registered.");

        if (await _context.AppUsers.AnyAsync(u => u.DocumentId == dto.DocumentId))
            return Conflict("This document is already registered.");

        var adminRole = await _context.AppRoles.FirstOrDefaultAsync(r => r.RoleName == "Administrator");
        if (adminRole == null)
            return StatusCode(500, "Administrator role not found in the database.");

        var user = new AppUser
        {
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            DocumentId = dto.DocumentId,
            BirthDate = dto.BirthDate,
            CreationDate = DateTime.UtcNow
        };

        var passwordHasher = new PasswordHasher<AppUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        var userRole = new AppUserRole
        {
            AppUser_ID = user.ID,
            AppRole_ID = adminRole.ID
        };

        _context.AppUserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateAdmin), new { id = user.ID }, user);
    }

}
