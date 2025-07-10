using System.ComponentModel.DataAnnotations;

public class CreateDimTransactionTypeDto
{
    [Required(ErrorMessage ="Description is required.")]
    [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters.")]
    public string Dim_Transaction_Type_Description { get; set; } = null!;
}
