using System;
using System.ComponentModel.DataAnnotations;

public class CreateAppUserDto
{
    [Required(ErrorMessage = "E-mail is required.")]
    [EmailAddress(ErrorMessage = "Invalid e-mail format.")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 255 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public string Password { get; set; } = null!;

    [RegularExpression(@"^[A-Za-zÀ-ú\s]+$", ErrorMessage = "Full name must contain only letters and spaces.")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number.")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Document is required.")]
    [StringLength(20)]
    public string DocumentId { get; set; } = null!;

    [Required(ErrorMessage = "Birth date is required.")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(CreateAppUserDto), nameof(ValidateBirthDate))]
    public DateTime BirthDate { get; set; }

    public static ValidationResult? ValidateBirthDate(DateTime birthDate, ValidationContext context)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age)) age--;

        return age >= 18
            ? ValidationResult.Success
            : new ValidationResult("User must be at least 18 years old.");
    }
}
