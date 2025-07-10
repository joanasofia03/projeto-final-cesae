public class ReadDimTimeDto
{
    public DateTime date_Date { get; set; }
    public int? date_Year { get; set; }
    public int? date_Month { get; set; }
    public int? date_Quarter { get; set; }
    public string? Weekday_Name { get; set; }
    public bool Is_Weekend { get; set; }
}
