public class ReadFactTransactionDto
{
    public string? SourceAccountName { get; set; }
    public string? DestinationAccountName { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? TransactionTypeName { get; set; }
    public string? AppUserName { get; set; }
    public decimal Transaction_Amount { get; set; }
    public decimal Balance_After_Transaction { get; set; }
    public string? Execution_Channel { get; set; }
    public string? Transaction_Status { get; set; }
}
