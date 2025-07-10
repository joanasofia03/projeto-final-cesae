using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Dim_Market_Asset
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [StringLength(100, ErrorMessage = "Asset name cannot exceed 100 characters.")]
    [RegularExpression(@"^[A-Za-z0-9\s\-\.]+$", ErrorMessage = "Asset name can contain only letters, numbers, spaces, hyphens, and dots.")]
    public string? Asset_Name { get; set; }

    [StringLength(30, ErrorMessage = "Asset type cannot exceed 30 characters.")]
    [RegularExpression(@"^[A-Za-z\s\-]+$", ErrorMessage = "Asset type must contain only letters, spaces, or hyphens.")]
    public string? Asset_Type { get; set; }

    [StringLength(10, ErrorMessage = "Symbol cannot exceed 10 characters.")]
    [RegularExpression(@"^[A-Z0-9\.]{1,10}$", ErrorMessage = "Symbol must be uppercase letters, numbers, or dots.")]
    public string? Symbol { get; set; }

    [StringLength(10, ErrorMessage = "Base currency cannot exceed 10 characters.")]
    [RegularExpression(@"^[A-Z]{3,10}$", ErrorMessage = "Base currency must be uppercase (e.g. USD, EUR, BTC).")]
    public string? Base_Currency { get; set; }

    [StringLength(100, ErrorMessage = "API source cannot exceed 100 characters.")]
    [RegularExpression(@"^[\w\s\-\.\/:]+$", ErrorMessage = "API source contains invalid characters.")]
    public string? API_Source { get; set; }

    public ICollection<Fact_Market_Asset_History>? MarketAssetHistories { get; set; }
}
