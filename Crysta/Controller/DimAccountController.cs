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
    private readonly ITransactionService _transactionService;
    private readonly INotificationService _notificationService;

    public Dim_AccountController(AnalyticPlatformContext context, ITransactionService transactionService, INotificationService notificationService)
    {
        _context = context;
        _transactionService = transactionService;
        _notificationService = notificationService;
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
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create([FromBody] CreateDimAccountDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        if (!User.IsInRole("Administrator"))
            return Forbid();

        if (dto.UserId <= 0)
            return BadRequest("UserId must be provided and greater than zero.");

        var account = new Dim_Account
        {
            Account_Type = dto.Account_Type,
            Account_Status = dto.Account_Status,
            AppUser_ID = dto.UserId,
            Opening_Date = DateTime.UtcNow,
            Currency = dto.Currency
        };

        _context.Dim_Accounts.Add(account);
        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            appUserId: dto.UserId,
            notificationDate: DateTime.UtcNow,
            notificationType: "Account Created",
            channel: "API",
            status: "Completed"
        );

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

        if (existing.AppUser_ID != userId && !User.IsInRole("Administrator"))
            return Forbid();

        existing.Account_Type = dto.Account_Type;
        existing.Account_Status = dto.Account_Status;

        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            appUserId: userId,
            notificationDate: DateTime.UtcNow,
            notificationType: "Account Updated",
            channel: "API",
            status: "Completed"
        );

        return NoContent();
    }

    // DELETE: http://localhost:5146/api/dim_account/delete/{id}
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);

        var account = await _context.Dim_Accounts.FindAsync(id);
        if (account == null)
            return NotFound("Account not found.");

        var balance = await _transactionService.GetAccountBalanceAsync(account.ID);

        if (balance != 0)
            return BadRequest($"Cannot delete account. Balance must be 0. Current balance: {balance}");

        _context.Dim_Accounts.Remove(account);
        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            appUserId: userId,
            notificationDate: DateTime.UtcNow,
            notificationType: "Account Deleted",
            channel: "API",
            status: "Completed"
        );

        return NoContent();
    }

    // GET: http://localhost:5146/api/dim_account/my-balance
    [HttpGet("my-balance")]
    [Authorize(Roles = "Client")]
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
            var balance = await _transactionService.GetAccountBalanceAsync(account.ID);

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
