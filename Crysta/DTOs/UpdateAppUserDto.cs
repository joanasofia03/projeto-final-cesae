using System.ComponentModel.DataAnnotations;

public class UpdateAppUserDto
{
    [Required(ErrorMessage = "E-mail is required.")]
    [EmailAddress(ErrorMessage = "Invalid e-mail format.")]
    [StringLength(100)]
    public string? Email { get; set; }

    [RegularExpression(@"^[A-Za-zÀ-ú\s]+$", ErrorMessage = "Full name must contain only letters and spaces.")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number.")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Region is required.")]
    [StringLength(50)]
    public string? Region { get; set; }

    [Required(ErrorMessage = "Birth date is required.")]
    [CustomValidation(typeof(AppUser), nameof(ValidateBirthDate))]
    public DateTime? BirthDate { get; set; }

    public static ValidationResult? ValidateBirthDate(DateTime birthDate, ValidationContext context)
    {
        var age = DateTime.Today.Year - birthDate.Year;
        if (birthDate > DateTime.Today.AddYears(-age)) age--;

        return age >= 18
            ? ValidationResult.Success
            : new ValidationResult("User must be at least 18 years old.");
    }
}
