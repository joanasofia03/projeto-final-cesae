using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Fact_Market_Asset_History
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    public int? Asset_ID { get; set; }
    public Dim_Market_Asset? Asset { get; set; }

    public int? Time_ID { get; set; }
    public Dim_Time? Time { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Open price must be a non-negative value.")]
    public decimal? Open_Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Close price must be a non-negative value.")]
    public decimal? Close_Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Trading volume must be a non-negative value.")]
    public decimal? Trading_Volume { get; set; }
}

/* var marketAssetHistory = new Fact_Market_Asset_History
{
    ID = 1001,
    Asset_ID = marketAsset.ID,
    Asset = marketAsset,
    Time_ID = time.ID,
    Time = time,
    Open_Price = 3100.25m,
    Close_Price = 3205.75m,
    Trading_Volume = 125000000.00m
}; */
