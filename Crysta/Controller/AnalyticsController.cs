using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;
    private readonly ITransactionService _transactionService;

    public AnalyticsController(AnalyticPlatformContext context, ITransactionService transactionService)
    {
        _context = context;
        _transactionService = transactionService;
    }

    // GET: http://localhost:5146/api/analytics/total-balance
    [HttpGet("total-balance")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetTotalBalance()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var accounts = await _context.Dim_Accounts
            .Where(a => a.AppUser_ID == userId)
            .ToListAsync();

        decimal total = 0;
        foreach (var account in accounts)
        {
            var balance = await _transactionService.GetAccountBalanceAsync(account.ID);
            total += balance;
        }

        return Ok(new { TotalBalance = total });
    }

    // GET: http://localhost:5146/api/analytics/average-spending
    [HttpGet("average-spending")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetAverageSpending()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var spending = await _context.Fact_Transactions
            .Where(t =>
                t.AppUser_ID == userId &&
                t.Source_Account_ID != null &&
                t.Source_Account_ID != t.Destination_Account_ID
            )
            .Include(t => t.Time)
            .ToListAsync();

        var monthlySpending = spending
            .GroupBy(t => new { t.Time.date_Year, t.Time.date_Month })
            .Select(g => new
            {
                Year = g.Key.date_Year,
                Month = g.Key.date_Month,
                TotalSpent = g.Sum(t => t.Transaction_Amount)
            })
            .ToList();

        var averageSpending = monthlySpending.Any()
            ? monthlySpending.Average(m => m.TotalSpent)
            : 0;

        return Ok(new
        {
            AverageSpending = averageSpending,
            MonthlyBreakdown = monthlySpending
        });
    }

    // GET: http://localhost:5146/api/analytics/balance-evolution
    [HttpGet("balance-evolution")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetBalanceEvolution()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var accounts = await _context.Dim_Accounts
            .Where(a => a.AppUser_ID == userId)
            .ToListAsync();

        if (!accounts.Any())
            return NotFound("No accounts found for user.");

        var allDates = await _context.Fact_Transactions
            .Where(t => accounts.Select(a => a.ID).Contains(t.Source_Account_ID) ||
                        accounts.Select(a => a.ID).Contains(t.Destination_Account_ID))
            .Include(t => t.Time)
            .Select(t => t.Time.date_Date)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync();

        var evolution = new List<object>();

        foreach (var date in allDates)
        {
            decimal totalBalance = 0;

            foreach (var account in accounts)
            {
                var balance = await _transactionService.GetBalanceAtAsync(account.ID, date);
                totalBalance += balance;
            }

            evolution.Add(new
            {
                Date = date,
                Balance = totalBalance
            });
        }

        return Ok(evolution);
    }
}
