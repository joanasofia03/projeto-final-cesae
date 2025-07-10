using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Dim_Transaction_Type
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters.")]
    public string? Dim_Transaction_Type_Description { get; set; }

    public ICollection<Fact_Transactions>? Transactions { get; set; }
}
