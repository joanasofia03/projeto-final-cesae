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
        var latestSourceTxn = await _context.Fact_Transactions
            .Where(t => t.Source_Account_ID == accountId)
            .OrderByDescending(t => t.ID)
            .FirstOrDefaultAsync();

        decimal balance;

        if (latestSourceTxn != null)
        {
            balance = latestSourceTxn.Balance_After_Transaction;

            var incomingAfter = await _context.Fact_Transactions
                .Where(t => t.Destination_Account_ID == accountId && t.ID > latestSourceTxn.ID)
                .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;

            balance += incomingAfter;
        }
        else
        {
            balance = await _context.Fact_Transactions
                .Where(t => t.Destination_Account_ID == accountId)
                .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;
        }

        return balance;
    }

    public async Task<decimal> GetBalanceAtAsync(int accountId, DateTime date)
    {
        var latestSourceTxn = await _context.Fact_Transactions
            .Where(t => t.Source_Account_ID == accountId && t.Time.date_Date <= date)
            .OrderByDescending(t => t.Time.date_Date)
            .ThenByDescending(t => t.ID)
            .FirstOrDefaultAsync();

        decimal balance;

        if (latestSourceTxn != null)
        {
            balance = latestSourceTxn.Balance_After_Transaction;

            var incomingAfter = await _context.Fact_Transactions
                .Include(t => t.Time)
                .Where(t => t.Destination_Account_ID == accountId &&
                            t.ID > latestSourceTxn.ID &&
                            t.Time.date_Date <= date)
                .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;

            balance += incomingAfter;
        }
        else
        {
            balance = await _context.Fact_Transactions
                .Include(t => t.Time)
                .Where(t => t.Destination_Account_ID == accountId && t.Time.date_Date <= date)
                .SumAsync(t => (decimal?)t.Transaction_Amount) ?? 0;
        }

        return balance;
    }

}
