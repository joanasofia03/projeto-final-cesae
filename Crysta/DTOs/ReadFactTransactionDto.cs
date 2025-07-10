public class ReadFactTransactionDto
{
    public int ID { get; set; }

    public int Source_Account_ID { get; set; }
    public string? SourceAccountName { get; set; }

    public int? Destination_Account_ID { get; set; }
    public string? DestinationAccountName { get; set; }

    public int? Time_ID { get; set; }
    public DateTime? TransactionDate { get; set; }

    public int Transaction_Type_ID { get; set; }
    public string? TransactionTypeName { get; set; }

    public int AppUser_ID { get; set; }
    public string? AppUserName { get; set; }

    public decimal Transaction_Amount { get; set; }

    public decimal Balance_After_Transaction { get; set; }

    public string? Execution_Channel { get; set; }

    public string Transaction_Status { get; set; }
}
