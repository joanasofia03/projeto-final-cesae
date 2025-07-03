using System;
using System.ComponentModel.DataAnnotations;

public class CreateAppUserDto
{
    [Required(ErrorMessage = "E-mail is required.")]
    [EmailAddress(ErrorMessage = "Invalid e-mail format.")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 255 characters.")]
    public string Password { get; set; } = null!;

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
    public DateTime BirthDate { get; set; }
}
