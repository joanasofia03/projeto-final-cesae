using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class Fact_Market_Asset_HistoryController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;

    public Fact_Market_Asset_HistoryController(AnalyticPlatformContext context)
    {
        _context = context;
    }

    // GET: http://localhost:5146/api/Fact_Market_Asset_History/getall/{assetId}
    [HttpGet("getall/{assetId}")]
    public async Task<ActionResult<IEnumerable<FactMarketAssetHistoryReadDto>>> GetAllByAssetId(int assetId)
    {
        var historyList = await _context.Fact_Market_Asset_History
            .Include(h => h.Time)
            .Where(h => h.Asset_ID == assetId)
            .OrderBy(h => h.Time.date_Date)
            .Select(h => new FactMarketAssetHistoryReadDto
            {
                Date = h.Time!.date_Date,
                Open_Price = h.Open_Price,
                Close_Price = h.Close_Price,
                Trading_Volume = h.Trading_Volume
            })
            .ToListAsync();

        if (!historyList.Any())
        {
            return NotFound($"No market asset history found for Asset_ID {assetId}");
        }

        return Ok(historyList);
    }
}
