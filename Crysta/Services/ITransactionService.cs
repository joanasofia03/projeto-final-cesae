public interface ITransactionService
{
    Task<decimal> GetAccountBalanceAsync(int accountId);
    Task<decimal> GetBalanceAtAsync(int accountId, DateTime date);
}
