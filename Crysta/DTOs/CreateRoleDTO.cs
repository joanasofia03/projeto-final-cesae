using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CreateRoleDto
{
    [Required(ErrorMessage = "Role name is required.")]
    [StringLength(50, ErrorMessage = "Role name must be at most 50 characters long.")]
    public string RoleName { get; set; } = null!;
}
