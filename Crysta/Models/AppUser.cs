using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AppUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required(ErrorMessage = "E-mail is required.")]
    [EmailAddress(ErrorMessage = "Invalid e-mail format.")]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; }

    [RegularExpression(@"^[A-Za-zÀ-ú\s]+$", ErrorMessage = "Full name must contain only letters and spaces.")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number.")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Document is required.")]
    [StringLength(20)]
    public string DocumentId { get; set; }

    public DateTime? DeletedAt { get; set; }

    [Required(ErrorMessage = "Birth date is required.")]
    [CustomValidation(typeof(AppUser), nameof(ValidateBirthDate))]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Region is required.")]
    [StringLength(50)]
    public string Region { get; set; } = null!;

    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    public ICollection<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();

    public static ValidationResult? ValidateBirthDate(DateTime birthDate, ValidationContext context)
    {
    var age = DateTime.Today.Year - birthDate.Year;
    if (birthDate > DateTime.Today.AddYears(-age)) age--;

    return age >= 18 
        ? ValidationResult.Success 
        : new ValidationResult("User must be at least 18 years old.");
    }
}
