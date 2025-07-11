public interface ITransactionService
{
    Task<decimal> GetAccountBalanceAsync(int accountId);
}
