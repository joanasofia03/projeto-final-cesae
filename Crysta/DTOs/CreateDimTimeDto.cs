using System;
using System.ComponentModel.DataAnnotations;

public class CreateDimTimeDto
{
    [Required(ErrorMessage = "Date is required.")]
    [DataType(DataType.Date)]
    public DateTime date_Date { get; set; }

    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
    public int? date_Year { get; set; }

    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
    public int? date_Month { get; set; }

    [Range(1, 4, ErrorMessage = "Quarter must be between 1 and 4.")]
    public int? date_Quarter { get; set; }

    [StringLength(10, ErrorMessage = "Weekday name cannot exceed 10 characters.")]
    [RegularExpression(@"^(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)?$",
        ErrorMessage = "Invalid weekday name.")]
    public string? Weekday_Name { get; set; }

    [Required(ErrorMessage = "IsWeekend is required.")]
    public bool Is_Weekend { get; set; }
}
