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

    // http://localhost:5146/api/roles
    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RoleName))
            return BadRequest("RoleName is required.");

        var exists = await _context.AppRoles.AnyAsync(r => r.RoleName == dto.RoleName);
        if (exists)
            return Conflict("Role already exists.");

        var role = new AppRole
        {
            RoleName = dto.RoleName
        };

        _context.AppRoles.Add(role);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoleById), new { id = role.RoleName }, role);
    }

    // http://localhost:5146/api/roles/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(int id)
    {
        var role = await _context.AppRoles.FindAsync(id);
        if (role == null)
            return NotFound();

        return Ok(role);
    }
}
