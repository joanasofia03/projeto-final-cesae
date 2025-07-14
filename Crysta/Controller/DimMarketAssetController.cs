using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class Dim_Market_AssetController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;

    public Dim_Market_AssetController(AnalyticPlatformContext context)
    {
        _context = context;
    }

    // GET: http://localhost:5146/api/dim_market_asset/getall
    [HttpGet("getall")]
    public async Task<ActionResult<List<DimMarketAssetReadDto>>> GetAll()
    {
        var assets = await _context.Dim_Market_Asset
            .AsNoTracking()
            .ToListAsync();

        var result = new List<DimMarketAssetReadDto>();

        foreach (var asset in assets)
        {
            result.Add(new DimMarketAssetReadDto
            {
                Asset_Name = asset.Asset_Name,
                Asset_Type = asset.Asset_Type,
                Symbol = asset.Symbol,
                Base_Currency = asset.Base_Currency,
                API_Source = asset.API_Source
            });
        }

        return Ok(result);
    }
}
