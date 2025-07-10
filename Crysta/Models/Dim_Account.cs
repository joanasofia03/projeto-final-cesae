using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

public class Dim_Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required(ErrorMessage = "Account type is required.")]
    [StringLength(30, ErrorMessage = "Account type cannot exceed 30 characters.")]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Account type must contain only letters, spaces, underscores or hyphens.")]
    public string? Account_Type { get; set; }

    [Required(ErrorMessage = "Account status is required.")]
    [StringLength(20, ErrorMessage = "Account status cannot exceed 20 characters.")]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Account status must contain only letters, spaces, underscores or hyphens.")]
    public string? Account_Status { get; set; }

    [Required(ErrorMessage = "AppUser_ID is required.")]
    public int? AppUser_ID { get; set; }
    public AppUser? AppUser { get; set; }

    [Required(ErrorMessage = "Opening date is required.")]
    [DataType(DataType.Date)]
    public DateTime? Opening_Date { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    [StringLength(10, ErrorMessage = "Currency code cannot exceed 10 characters.")]
    [RegularExpression(@"^[A-Z]{3,10}$", ErrorMessage = "Currency should be in uppercase and between 3 to 10 characters.")]
    public string? Currency { get; set; }

    public ICollection<Fact_Transactions>? SourceTransactions { get; set; }
    public ICollection<Fact_Transactions>? DestinationTransactions { get; set; }
}

// {
//   "Account_Type": "Savings",
//   "Account_Status": "Active",
//   "AppUser_ID": 12,
//   "Opening_Date": "2023-09-01T00:00:00",
//   "Currency": "EUR"
// }
