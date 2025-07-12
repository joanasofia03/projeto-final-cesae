public class MarketChartRangeResponse
{
    public List<List<double>> prices { get; set; } = new();
    public List<List<double>> total_volumes { get; set; } = new();
}
