using System;
using System.ComponentModel.DataAnnotations;

public class AppUser
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "E-mail is required.")]
    [EmailAddress(ErrorMessage = "Invalid e-mail format.")]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; }

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
    public DateTime BirthDate { get; set; }

    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    public ICollection<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();
}
