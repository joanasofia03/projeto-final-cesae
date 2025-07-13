using Microsoft.EntityFrameworkCore;

public class TransactionService : ITransactionService
{
    private readonly AnalyticPlatformContext _context;

    public TransactionService(AnalyticPlatformContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetAccountBalanceAsync(int accountId)
    {
        var deposits = await _context.Fact_Transactions
            .Where(t => t.Source_Account_ID == accountId && t.Destination_Account_ID == accountId)
            .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;
        
        var incoming = await _context.Fact_Transactions
            .Where(t => t.Destination_Account_ID == accountId && t.Source_Account_ID != accountId)
            .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;

        var outgoing = await _context.Fact_Transactions
            .Where(t => t.Source_Account_ID == accountId && t.Destination_Account_ID != accountId)
            .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;

        return deposits + incoming - outgoing;
    }

    public async Task<decimal> GetBalanceAtAsync(int accountId, DateTime date)
    {
        var deposits = await _context.Fact_Transactions
            .Include(t => t.Time)
            .Where(t => t.Source_Account_ID == accountId &&
                        t.Destination_Account_ID == accountId &&
                        t.Time.date_Date <= date)
            .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;

        var incoming = await _context.Fact_Transactions
            .Include(t => t.Time)
            .Where(t => t.Destination_Account_ID == accountId &&
                        t.Source_Account_ID != accountId &&
                        t.Time.date_Date <= date)
            .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;

        var outgoing = await _context.Fact_Transactions
            .Include(t => t.Time)
            .Where(t => t.Source_Account_ID == accountId &&
                        t.Destination_Account_ID != accountId &&
                        t.Time.date_Date <= date)
            .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;

        return deposits + incoming - outgoing;
    }

}
