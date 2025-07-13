using System;
using System.ComponentModel.DataAnnotations;

public class CreateDimAccountDto
{
    [Required(ErrorMessage = "Account type is required.")]
    [StringLength(30)]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Account type must contain only letters, spaces, underscores or hyphens.")]
    public string? Account_Type { get; set; }

    [Required(ErrorMessage = "Account status is required.")]
    [StringLength(20)]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Account status must contain only letters, spaces, underscores or hyphens.")]
    public string? Account_Status { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    [StringLength(10)]
    [RegularExpression(@"^[A-Z]{3,10}$", ErrorMessage = "Currency should be in uppercase and between 3 to 10 characters.")]
    public string? Currency { get; set; }
}
