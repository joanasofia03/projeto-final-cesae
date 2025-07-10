using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class Dim_AccountController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;

    public Dim_AccountController(AnalyticPlatformContext context)
    {
        _context = context;
    }

    // GET: http://localhost:5146/api/dim_account/getall
    [HttpGet("getall")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAll()
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminRole = await _context.AppRoles.FirstOrDefaultAsync(r => r.RoleName == "Administrator");
        if (adminRole == null)
            return StatusCode(500, "Administrator role not found in the database.");

        var accounts = await _context.Set<Dim_Account>()
            .Select(a => new ReadDimAccountDto
            {
                Account_Type = a.Account_Type,
                Account_Status = a.Account_Status,
                AppUser_ID = a.AppUser_ID,
                Opening_Date = a.Opening_Date,
                Currency = a.Currency
            })
            .ToListAsync();

        return Ok(accounts);
    }

    // GET: http://localhost:5146/api/dim_account/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var account = await _context.Set<Dim_Account>().FindAsync(id);
        if (account == null)
            return NotFound();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);

        if (account.AppUser_ID != userId)
            return Forbid();

        var dto = new ReadDimAccountDto
        {
            Account_Type = account.Account_Type,
            Account_Status = account.Account_Status,
            AppUser_ID = account.AppUser_ID,
            Opening_Date = account.Opening_Date,
            Currency = account.Currency
        };

        return Ok(dto);
    }

    // POST: http://localhost:5146/api/dim_account/create-account
    [HttpPost("create-account")]
    public async Task<IActionResult> Create([FromBody] CreateDimAccountDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
            return Unauthorized();

        var userId = int.Parse(userIdString);

        var account = new Dim_Account
        {
            Account_Type = dto.Account_Type,
            Account_Status = dto.Account_Status,
            AppUser_ID = userId, // LoggedUser
            Opening_Date = dto.Opening_Date,
            Currency = dto.Currency
        };

        _context.Dim_Accounts.Add(account);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = account.ID }, account);
    }

    // PUT: http://localhost:5146/api/dim_account/update/{id}
    [HttpPut("update/{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDimAccountDto dto)
    {
        var existing = await _context.Set<Dim_Account>().FindAsync(id);
        if (existing == null)
            return NotFound();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);

        if (existing.AppUser_ID != userId)
            return Forbid();

        existing.Account_Type = dto.Account_Type;
        existing.Account_Status = dto.Account_Status;
        existing.Opening_Date = dto.Opening_Date;
        existing.Currency = dto.Currency;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: http://localhost:5146/api/dim_account/delete/{id}
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminRole = await _context.AppRoles.FirstOrDefaultAsync(r => r.RoleName == "Administrator");
        if (adminRole == null)
            return StatusCode(500, "Administrator role not found in the database.");

        var existing = await _context.Set<Dim_Account>().FindAsync(id);
        if (existing == null)
            return NotFound();

        _context.Set<Dim_Account>().Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: http://localhost:5146/api/dim_account/my-balance
    [HttpGet("my-balance")]
    public async Task<IActionResult> GetMyBalance()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);

        var accounts = await _context.Dim_Accounts
            .Where(a => a.AppUser_ID == userId)
            .ToListAsync();

        if (!accounts.Any())
            return NotFound("No accounts found for the current user.");

        var accountBalances = new List<object>();

        foreach (var account in accounts)
        {
            var latestSourceTxn = await _context.Fact_Transactions
                .Where(t => t.Source_Account_ID == account.ID)
                .OrderByDescending(t => t.ID)
                .FirstOrDefaultAsync();

            decimal balance;

            if (latestSourceTxn != null)
            {
                // Start with the known balance from last sent transaction
                balance = latestSourceTxn.Balance_After_Transaction;

                // Add any received funds after that transaction
                var incomingAfterSource = await _context.Fact_Transactions
                    .Where(t => t.Destination_Account_ID == account.ID && t.ID > latestSourceTxn.ID)
                    .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;

                balance += incomingAfterSource;
            }
            else
            {
                // No outgoing transactions — calculate total received
                balance = await _context.Fact_Transactions
                    .Where(t => t.Destination_Account_ID == account.ID)
                    .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;
            }

            accountBalances.Add(new
            {
                AccountId = account.ID,
                AccountType = account.Account_Type,
                Currency = account.Currency,
                Balance = balance
            });
        }

        return Ok(accountBalances);
    }

}
