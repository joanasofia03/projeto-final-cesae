using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class Dim_Transaction_TypeController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;

    public Dim_Transaction_TypeController(AnalyticPlatformContext context)
    {
        _context = context;
    }

    // GET: http://localhost:5146/api/dim_transaction_type/getall
    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var types = await _context.Set<Dim_Transaction_Type>()
            .Select(t => new ReadDimTransactionTypeDto
            {
                Dim_Transaction_Type_Description = t.Dim_Transaction_Type_Description ?? string.Empty
            })
            .ToListAsync();

        return Ok(types);
    }

    // GET: http://localhost:5146/api/dim_transaction_type/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var type = await _context.Set<Dim_Transaction_Type>().FindAsync(id);
        if (type == null)
            return NotFound();

        var dto = new ReadDimTransactionTypeDto
        {
            Dim_Transaction_Type_Description = type.Dim_Transaction_Type_Description ?? string.Empty
        };

        return Ok(dto);
    }

    // POST: http://localhost:5146/api/dim_transaction_type/create-transactiontype
    [HttpPost("create-transactiontype")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create([FromBody] CreateDimTransactionTypeDto dto)
    {
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminRole = await _context.AppRoles.FirstOrDefaultAsync(r => r.RoleName == "Administrator");
        if (adminRole == null)
            return StatusCode(500, "Administrator role not found in the database.");

        var entity = new Dim_Transaction_Type
        {
            Dim_Transaction_Type_Description = dto.Dim_Transaction_Type_Description
        };

        _context.Set<Dim_Transaction_Type>().Add(entity);
        await _context.SaveChangesAsync();

        var resultDto = new ReadDimTransactionTypeDto
        {
            Dim_Transaction_Type_Description = entity.Dim_Transaction_Type_Description ?? string.Empty
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.ID }, resultDto);
    }

    // DELETE: http://localhost:5146/api/dim_transaction_type/delete/{id}
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminRole = await _context.AppRoles.FirstOrDefaultAsync(r => r.RoleName == "Administrator");
        if (adminRole == null)
            return StatusCode(500, "Administrator role not found in the database.");

        var existing = await _context.Set<Dim_Transaction_Type>().FindAsync(id);
        if (existing == null)
            return NotFound();

        _context.Set<Dim_Transaction_Type>().Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
