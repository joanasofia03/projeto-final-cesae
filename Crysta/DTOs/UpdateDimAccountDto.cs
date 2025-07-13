using System;
using System.ComponentModel.DataAnnotations;

public class UpdateDimAccountDto
{
    [Required(ErrorMessage = "Account type is required.")]
    [StringLength(30)]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Account type must contain only letters, spaces, underscores or hyphens.")]
    public string? Account_Type { get; set; }

    [Required(ErrorMessage = "Account status is required.")]
    [StringLength(20)]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Account status must contain only letters, spaces, underscores or hyphens.")]
    public string? Account_Status { get; set; }

    public int? AppUser_ID { get; set; }
}
