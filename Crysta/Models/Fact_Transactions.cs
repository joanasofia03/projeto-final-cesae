using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Fact_Transactions
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required(ErrorMessage = "Source Account ID is required.")]
    public int Source_Account_ID { get; set; }

    public Dim_Account SourceAccount { get; set; } = null!;

    [Required(ErrorMessage = "Destination Account ID is required.")]
    public int Destination_Account_ID { get; set; }

    public Dim_Account? DestinationAccount { get; set; }

    public int? Time_ID { get; set; }

    public Dim_Time? Time { get; set; }

    [Required(ErrorMessage = "Transaction Type ID is required.")]
    public int Transaction_Type_ID { get; set; }

    public Dim_Transaction_Type TransactionType { get; set; } = null!;

    [Required(ErrorMessage = "App User ID is required.")]
    public int AppUser_ID { get; set; }

    public AppUser AppUser { get; set; } = null!;

    [Required(ErrorMessage = "Transaction amount is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Transaction amount must be zero or greater.")]
    public decimal Transaction_Amount { get; set; }

    [Required(ErrorMessage = "Balance after transaction is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Balance after transaction must be zero or greater.")]
    public decimal Balance_After_Transaction { get; set; }

    [StringLength(50, ErrorMessage = "Execution channel cannot exceed 50 characters.")]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Execution channel must contain only letters, spaces, hyphens, or underscores.")]
    public string? Execution_Channel { get; set; }

    [Required(ErrorMessage = "Transaction status is required.")]
    [StringLength(30, ErrorMessage = "Transaction status cannot exceed 30 characters.")]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Transaction status must contain only letters, spaces, hyphens, or underscores.")]
    public string Transaction_Status { get; set; } = null!;
}
