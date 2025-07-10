using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class Dim_TimeController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;

    public Dim_TimeController(AnalyticPlatformContext context)
    {
        _context = context;
    }

    // GET: http://localhost:5146/api/dim_time/getall
    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var times = await _context.Dim_Time
            .Select(t => new ReadDimTimeDto
            {
                date_Date = t.date_Date,
                date_Year = t.date_Year,
                date_Month = t.date_Month,
                date_Quarter = t.date_Quarter,
                Weekday_Name = t.Weekday_Name,
                Is_Weekend = t.Is_Weekend
            })
            .ToListAsync();

        return Ok(times);
    }

    // GET: http://localhost:5146/api/dim_time/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var time = await _context.Dim_Time.FindAsync(id);
        if (time == null)
            return NotFound();

        var dto = new ReadDimTimeDto
        {
            date_Date = time.date_Date,
            date_Year = time.date_Year,
            date_Month = time.date_Month,
            date_Quarter = time.date_Quarter,
            Weekday_Name = time.Weekday_Name,
            Is_Weekend = time.Is_Weekend
        };

        return Ok(dto);
    }

    // POST: http://localhost:5146/api/dim_time/create-time
    [HttpPost("create-time")]
    public async Task<IActionResult> Create([FromBody] CreateDimTimeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var dimTime = new Dim_Time
        {
            date_Date = dto.date_Date,
            date_Year = dto.date_Date.Year,
            date_Month = dto.date_Date.Month,
            date_Quarter = (dto.date_Date.Month - 1) / 3 + 1,
            Weekday_Name = dto.date_Date.DayOfWeek.ToString(),
            Is_Weekend = dto.date_Date.DayOfWeek == DayOfWeek.Saturday || dto.date_Date.DayOfWeek == DayOfWeek.Sunday
        };

        _context.Dim_Time.Add(dimTime);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = dimTime.ID }, new ReadDimTimeDto
        {
            date_Date = dimTime.date_Date,
            date_Year = dimTime.date_Year,
            date_Month = dimTime.date_Month,
            date_Quarter = dimTime.date_Quarter,
            Weekday_Name = dimTime.Weekday_Name,
            Is_Weekend = dimTime.Is_Weekend
        });
    }

    // DELETE: http://localhost:5146/api/dim_time/delete/5
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _context.Dim_Time.FindAsync(id);
        if (existing == null)
            return NotFound();

        _context.Dim_Time.Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
