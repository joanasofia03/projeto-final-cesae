using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class Fact_TransactionsController : ControllerBase
{
    private readonly AnalyticPlatformContext _context;

    public Fact_TransactionsController(AnalyticPlatformContext context)
    {
        _context = context;
    }

    // GET: http://localhost:5146/api/fact_transactions/getall
    [HttpGet("getall")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAll()
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminRole = await _context.AppRoles.FirstOrDefaultAsync(r => r.RoleName == "Administrator");
        if (adminRole == null)
            return StatusCode(500, "Administrator role not found in the database.");

        var transactions = await _context.Fact_Transactions
            .Include(t => t.SourceAccount)
            .Include(t => t.DestinationAccount)
            .Include(t => t.Time)
            .Include(t => t.TransactionType)
            .Include(t => t.AppUser)
            .Select(t => new ReadFactTransactionDto
            {
                SourceAccountName = t.SourceAccount.AppUser.FullName,
                DestinationAccountName = t.DestinationAccount.AppUser.FullName,
                TransactionDate = t.Time != null ? t.Time.date_Date : null,
                TransactionTypeName = t.TransactionType.Dim_Transaction_Type_Description,
                AppUserName = t.AppUser.FullName,
                Transaction_Amount = t.Transaction_Amount,
                Balance_After_Transaction = t.Balance_After_Transaction,
                Execution_Channel = t.Execution_Channel,
                Transaction_Status = t.Transaction_Status
            })
            .ToListAsync();

        return Ok(transactions);
    }

    // GET: http://localhost:5146/api/fact_transactions/getmine
    [HttpGet("getmine")]
    public async Task<IActionResult> GetMine()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var transactions = await _context.Fact_Transactions
            .Where(t => t.AppUser_ID == userId)
            .Include(t => t.SourceAccount)
            .Include(t => t.DestinationAccount)
            .Include(t => t.Time)
            .Include(t => t.TransactionType)
            .Include(t => t.AppUser)
            .Select(t => new ReadFactTransactionDto
            {
                SourceAccountName = t.SourceAccount.AppUser.FullName,
                DestinationAccountName = t.DestinationAccount.AppUser.FullName,
                TransactionDate = t.Time != null ? t.Time.date_Date : null,
                TransactionTypeName = t.TransactionType.Dim_Transaction_Type_Description,
                AppUserName = t.AppUser.FullName,
                Transaction_Amount = t.Transaction_Amount,
                Balance_After_Transaction = t.Balance_After_Transaction,
                Execution_Channel = t.Execution_Channel,
                Transaction_Status = t.Transaction_Status
            })
            .ToListAsync();

        return Ok(transactions);
    }

    // POST: http://localhost:5146/api/fact_transactions/create-transaction
    [HttpPost("create-transaction")]
    public async Task<IActionResult> Create([FromBody] FactTransactionCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        if (dto.AppUser_ID != userId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                Error = "Forbidden",
                Message = "You can only create transactions for yourself."
            });
        }

        var sourceAccountExists = await _context.Dim_Accounts.AnyAsync(a => a.ID == dto.Source_Account_ID);
        if (!sourceAccountExists)
            return BadRequest(new { Error = "Invalid Source_Account_ID", Message = "No Dim_Account found with mentioned ID" });

        var destAccountExists = await _context.Dim_Accounts.AnyAsync(a => a.ID == dto.Destination_Account_ID);
        if (!destAccountExists)
            return BadRequest(new { Error = "Invalid Destination_Account_ID", Message = "No Dim_Account found with mentioned ID" });

        var transactionTypeExists = await _context.Dim_Transaction_Types.AnyAsync(t => t.ID == dto.Transaction_Type_ID);
        if (!transactionTypeExists)
            return BadRequest(new { Error = "Invalid Transaction_Type_ID", Message = "No Dim_Transaction_Type found with mentioned ID" });

        var now = DateTime.UtcNow;
        var dimTime = new Dim_Time
        {
            date_Date = now,
            date_Year = now.Year,
            date_Month = now.Month,
            date_Quarter = (now.Month - 1) / 3 + 1,
            Weekday_Name = now.DayOfWeek.ToString(),
            Is_Weekend = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday
        };

        _context.Dim_Time.Add(dimTime);
        await _context.SaveChangesAsync();

        var entity = new Fact_Transactions
        {
            Source_Account_ID = dto.Source_Account_ID,
            Destination_Account_ID = dto.Destination_Account_ID,
            Time_ID = dimTime.ID,
            Transaction_Type_ID = dto.Transaction_Type_ID,
            AppUser_ID = userId,
            Transaction_Amount = dto.Transaction_Amount,
            Balance_After_Transaction = dto.Balance_After_Transaction,
            Execution_Channel = dto.Execution_Channel,
            Transaction_Status = dto.Transaction_Status
        };

        _context.Fact_Transactions.Add(entity);
        await _context.SaveChangesAsync();

        var createdDto = await _context.Fact_Transactions
            .Where(t => t.ID == entity.ID)
            .Include(t => t.SourceAccount)
            .Include(t => t.DestinationAccount)
            .Include(t => t.Time)
            .Include(t => t.TransactionType)
            .Include(t => t.AppUser)
            .Select(t => new ReadFactTransactionDto
            {
                SourceAccountName = t.SourceAccount.AppUser.FullName,
                DestinationAccountName = t.DestinationAccount.AppUser.FullName,
                TransactionDate = t.Time != null ? t.Time.date_Date : null,
                TransactionTypeName = t.TransactionType.Dim_Transaction_Type_Description,
                AppUserName = t.AppUser.FullName,
                Transaction_Amount = t.Transaction_Amount,
                Balance_After_Transaction = t.Balance_After_Transaction,
                Execution_Channel = t.Execution_Channel,
                Transaction_Status = t.Transaction_Status
            })
            .FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetMine), new { id = entity.ID }, createdDto);
    }

    // GET: http://localhost:5146/api/fact_transactions/filter?from=2025-07-01&to=2025-07-11&type=1
    // GET: http://localhost:5146/api/fact_transactions/filter?from=2025-07-10
    // GET: http://localhost:5146/api/fact_transactions/filter?type=1
    [HttpGet("filter")]
    public async Task<IActionResult> FilterTransactions(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int? type)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();

        var userAccountIds = await _context.Dim_Accounts
            .Where(a => a.AppUser_ID == userId)
            .Select(a => a.ID)
            .ToListAsync();

        var query = _context.Fact_Transactions
            .Include(t => t.SourceAccount).ThenInclude(a => a.AppUser)
            .Include(t => t.DestinationAccount).ThenInclude(a => a.AppUser)
            .Include(t => t.Time)
            .Include(t => t.TransactionType)
            .Include(t => t.AppUser)
            .Where(t =>
            userAccountIds.Contains(t.Source_Account_ID) ||
            userAccountIds.Contains(t.Destination_Account_ID)
        );

        if (type.HasValue)
            query = query.Where(t => t.Transaction_Type_ID == type.Value);

        if (from.HasValue)
            query = query.Where(t => t.Time != null && t.Time.date_Date.Date >= from.Value.Date);

        if (to.HasValue)
            query = query.Where(t => t.Time != null && t.Time.date_Date.Date <= to.Value.Date);

        var transactions = await query
            .Select(t => new ReadFactTransactionDto
            {
                SourceAccountName = t.SourceAccount.AppUser.FullName,
                DestinationAccountName = t.DestinationAccount.AppUser.FullName,
                TransactionDate = t.Time != null ? t.Time.date_Date : null,
                TransactionTypeName = t.TransactionType.Dim_Transaction_Type_Description,
                AppUserName = t.AppUser.FullName,
                Transaction_Amount = t.Transaction_Amount,
                Balance_After_Transaction = t.Balance_After_Transaction,
                Execution_Channel = t.Execution_Channel,
                Transaction_Status = t.Transaction_Status
            })
            .ToListAsync();

        if (!transactions.Any())
        {
            return NotFound(new
            {
                Message = "No transactions found matching the specified filters."
            });
        }

        return Ok(transactions);
    }
}
