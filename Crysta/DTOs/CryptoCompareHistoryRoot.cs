public class CryptoCompareHistoryRoot
{
    public CryptoCompareDataWrapper Data { get; set; }
}

public class CryptoCompareDataWrapper
{
    public List<CryptoCompareHistoryEntry> Data { get; set; }
}

public class CryptoCompareHistoryEntry
{
    public long time { get; set; }
    public double open { get; set; }
    public double close { get; set; }
    public double volumeto { get; set; }
}
