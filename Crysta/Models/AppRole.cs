using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AppRole
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required(ErrorMessage = "Role name is required.")]
    [StringLength(50, ErrorMessage = "Role name must be at most 50 characters long.")]
    public string RoleName { get; set; } = null!;

    public ICollection<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();
}
