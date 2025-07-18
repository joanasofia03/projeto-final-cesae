using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;
    private readonly PasswordHasher<AppUser> _passwordHasher;
    private readonly INotificationService _notificationService;

    public UsersController(AnalyticPlatformContext context, INotificationService notificationService)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<AppUser>();
        _notificationService = notificationService;
    }

    // GET: http://localhost:5146/api/users/getall
    [HttpGet("getall")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.AppUsers
            .Where(u => u.DeletedAt == null)
            .Select(u => new
            {
                u.ID,
                u.Email,
                u.FullName,
                u.PhoneNumber,
                u.DocumentId,
                u.BirthDate,
                u.Region
            })
            .ToListAsync();

        if (users == null || !users.Any())
            return NotFound("No users found.");

        return Ok(users);
    }

    // POST http://localhost:5146/api/users/create-user
    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateAppUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.AppUsers.AnyAsync(u => u.Email == dto.Email))
            return Conflict("This email is already registered.");

        if (await _context.AppUsers.AnyAsync(u => u.DocumentId == dto.DocumentId))
            return Conflict("This document is already registered.");

        if (await _context.AppUsers.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber))
            return Conflict("This phone number is already registered.");

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
            Region = dto.Region,
            CreationDate = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        var userRole = new AppUserRole
        {
            AppUser_ID = user.ID,
            AppRole_ID = clientRole.ID
        };

        _context.AppUserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.ID }, new
        {
            user.ID,
            user.Email,
            user.FullName,
            user.PhoneNumber,
            user.DocumentId,
            user.BirthDate
        });
    }

    // GET http://localhost:5146/api/users/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _context.AppUsers.FindAsync(id);

        if (user == null || user.DeletedAt != null)
            return NotFound();

        return Ok(new
        {
            user.ID,
            user.Email,
            user.FullName,
            user.PhoneNumber,
            user.DocumentId,
            user.BirthDate,
            user.Region
        });
    }

    // GET http://localhost:5146/api/users/me
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var user = await _context.AppUsers
            .Include(u => u.AppUserRoles)
                .ThenInclude(ur => ur.AppRole)
            .FirstOrDefaultAsync(u => u.ID == int.Parse(userId));

        if (user == null)
            return NotFound();

        var roles = user.AppUserRoles.Select(r => r.AppRole.RoleName).ToList();

        var response = new UserLoginResponseDto
        {
            Id = user.ID,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            DocumentId = user.DocumentId ?? string.Empty,
            BirthDate = user.BirthDate,
            Region = user.Region ?? string.Empty,
            Roles = roles,
            Token = ""
        };

        return Ok(response);
    }

    // PUT http://localhost:5146/api/users/update-user/{id}
    [Authorize]
    [HttpPut("update-user/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateAppUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var currentUserId = int.Parse(userId);
        var isAdmin = User.IsInRole("Administrator");

        if (currentUserId != id && !isAdmin)
            return Forbid("You are not authorized to update this user.");

        var user = await _context.AppUsers.FindAsync(id);
        if (user == null || user.DeletedAt != null)
            return NotFound("User not found.");

        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email;

        if (!string.IsNullOrWhiteSpace(dto.FullName))
            user.FullName = dto.FullName;

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            user.PhoneNumber = dto.PhoneNumber;

        if (!string.IsNullOrWhiteSpace(dto.Region))
            user.Region = dto.Region;

        if (dto.BirthDate != null)
            user.BirthDate = (DateTime)dto.BirthDate;

        if (await _context.AppUsers.AnyAsync(u => u.Email == user.Email && u.ID != id))
            return Conflict("This email is already registered.");

        if (await _context.AppUsers.AnyAsync(u => u.PhoneNumber == user.PhoneNumber && u.ID != id))
            return Conflict("This phone number is already registered.");

        _context.AppUsers.Update(user);
        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
        appUserId: user.ID,
        notificationDate: DateTime.UtcNow,
        notificationType: "Account Information Updated",
        channel: "API",
        status: "Completed"
        );

        return Ok(new
        {
            message = "User updated successfully.",
            user.ID,
            user.Email,
            user.FullName,
            user.PhoneNumber,
            user.Region,
            user.BirthDate
        });
    }

    // PUT http://localhost:5146/api/users/update-password/{id}
    [Authorize]
    [HttpPut("update-password/{id}")]
    public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var currentUserId = int.Parse(userId);
        var isAdmin = User.IsInRole("Administrator");

        if (currentUserId != id && !isAdmin)
            return Forbid("You are not authorized to change this user's password.");

        var user = await _context.AppUsers.FindAsync(id);
        if (user == null || user.DeletedAt != null)
            return NotFound("User not found.");

        if (!isAdmin)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Current password is incorrect.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

        _context.AppUsers.Update(user);
        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            appUserId: user.ID,
            notificationDate: DateTime.UtcNow,
            notificationType: "Password Change",
            channel: "API",
            status: "Completed"
        );

        return Ok("Password updated successfully.");
    }


}