using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;

    public RolesController(AnalyticPlatformContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.role_name))
            return BadRequest("role_name is required.");

        var exists = await _context.AppRoles.AnyAsync(r => r.role_name == dto.role_name);
        if (exists)
            return Conflict("Role already exists.");

        var role = new AppRole
        {
            role_name = dto.role_name
        };

        _context.AppRoles.Add(role);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoleById), new { id = role.id }, role);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(int id)
    {
        var role = await _context.AppRoles.FindAsync(id);
        if (role == null)
            return NotFound();

        return Ok(role);
    }
}
