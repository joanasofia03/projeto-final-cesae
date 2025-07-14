using System.Net.Http;
using System.Text.Json;

public static class DataSeeder
{
    public static async Task SeedMarketAssetsFromCoinGecko(AnalyticPlatformContext context)
    {
        using var httpClient = new HttpClient();

        var response = await httpClient.GetAsync("https://api.coingecko.com/api/v3/coins/list");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Failed to fetch assets from CoinGecko.");
            return;
        }

        var json = await response.Content.ReadAsStringAsync();
        var assets = JsonSerializer.Deserialize<List<CoinGeckoAssetDto>>(json);

        if (assets == null || !assets.Any())
        {
            Console.WriteLine("No assets received from CoinGecko.");
            return;
        }

        var bitcoin = assets
            .FirstOrDefault(a =>
                a.id == "bitcoin" &&
                !string.IsNullOrWhiteSpace(a.symbol) &&
                !string.IsNullOrWhiteSpace(a.name));

        if (bitcoin == null)
        {
            Console.WriteLine("Bitcoin not found in the CoinGecko asset list.");
            return;
        }

        var exists = context.Dim_Market_Asset.Any(a => a.Symbol == bitcoin.symbol.ToUpper());
        if (exists)
        {
            Console.WriteLine("Bitcoin already exists in the database.");
            return;
        }

        var newAsset = new Dim_Market_Asset
        {
            Asset_Name = bitcoin.name,
            Asset_Type = "Cryptocurrency",
            Symbol = bitcoin.symbol.ToUpper(),
            Base_Currency = "USD",
            API_Source = $"https://min-api.cryptocompare.com/data/v2/histoday?fsym={bitcoin.symbol.ToUpper()}&tsym=USD"
        };

        context.Dim_Market_Asset.Add(newAsset);
        await context.SaveChangesAsync();

        Console.WriteLine("Seeded Bitcoin asset from CoinGecko.");
    }

    /* public static async Task SeedFactMarketAssetHistoryAsync(AnalyticPlatformContext context, Dim_Market_Asset asset, Dim_Time timeStart, Dim_Time timeEnd)
    {
        using var httpClient = new HttpClient();

        // Convert to UNIX timestamps (seconds)
        long from = new DateTimeOffset(new DateTime(2025, 1, 1)).ToUnixTimeSeconds();
        long to = new DateTimeOffset(new DateTime(2025, 6, 30, 23, 59, 59)).ToUnixTimeSeconds();

        string url = $"https://api.coingecko.com/api/v3/coins/{asset.Asset_Name!.ToLower()}/market_chart/range?vs_currency={asset.Base_Currency!.ToLower()}&from={from}&to={to}";
        Console.WriteLine($"Fetching market data from: {url}");
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch market data: {response.StatusCode}");
            return;
        }

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        var prices = doc.RootElement.GetProperty("prices");         // [[timestamp_ms, price], ...]
        var volumes = doc.RootElement.GetProperty("total_volumes");  // [[timestamp_ms, volume], ...]

        // We will create one Fact_Market_Asset_History entry per day (timestamp_ms is in milliseconds)
        var existingTimes = context.Dim_Time.ToDictionary(t => t.date_Date.Date);

        foreach (var priceEntry in prices.EnumerateArray())
        {
            long timestampMs = priceEntry[0].GetInt64();
            decimal price = priceEntry[1].GetDecimal();

            // Convert timestamp to DateTime (UTC)
            DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).UtcDateTime.Date;

            // Check if we have the time dimension already or create it
            if (!existingTimes.TryGetValue(date, out Dim_Time? timeDimension))
            {
                timeDimension = new Dim_Time
                {
                    date_Date = date,
                    date_Year = date.Year,
                    date_Month = date.Month,
                    date_Quarter = (date.Month - 1) / 3 + 1,
                    Weekday_Name = date.DayOfWeek.ToString(),
                    Is_Weekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday
                };
                context.Dim_Time.Add(timeDimension);
                context.SaveChanges();
                existingTimes[date] = timeDimension;
            }

            // Find matching volume for the day
            decimal volume = 0m;

            var volumeEntry = volumes.EnumerateArray()
                .FirstOrDefault(v =>
                    DateTimeOffset.FromUnixTimeMilliseconds(v[0].GetInt64()).UtcDateTime.Date == date);

            if (volumeEntry.ValueKind != JsonValueKind.Undefined)
            {
                volume = volumeEntry[1].GetDecimal();
            }

            // For simplicity, we'll treat Open_Price and Close_Price as equal to 'price' (approximate)
            var history = new Fact_Market_Asset_History
            {
                Asset_ID = asset.ID,
                Time_ID = timeDimension.ID,
                Open_Price = price,
                Close_Price = price,
                Trading_Volume = volume
            };

            context.Fact_Market_Asset_History.Add(history);
        }

        await context.SaveChangesAsync();

        Console.WriteLine("Seeded Fact_Market_Asset_History from CoinGecko successfully.");
    } */

    public static async Task SeedFactMarketAssetHistory(AnalyticPlatformContext context)
    {
        if (context.Fact_Market_Asset_History.Any())
        {
        Console.WriteLine("Fact_Market_Asset_History already contains data. Skipping seeding.");
        return;
        }

        var asset = context.Dim_Market_Asset.FirstOrDefault(a => a.Symbol == "BTC");
        if (asset == null)
        {
            Console.WriteLine("Bitcoin asset not found in Dim_Market_Asset.");
            return;
        }

        using var httpClient = new HttpClient();

        var toTs = new DateTimeOffset(new DateTime(2025, 7, 1)).ToUnixTimeSeconds();
        var url = $"https://min-api.cryptocompare.com/data/v2/histoday?fsym=BTC&tsym=USD&limit=180&toTs={toTs}";

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch data: {response.StatusCode}");
            return;
        }

        var json = await response.Content.ReadAsStringAsync();
        var root = JsonSerializer.Deserialize<CryptoCompareHistoryRoot>(json);

        if (root?.Data?.Data == null)
        {
            Console.WriteLine("No data returned.");
            return;
        }

        foreach (var entry in root.Data.Data)
        {
            var date = DateTimeOffset.FromUnixTimeSeconds(entry.time).Date;

            var timeEntry = context.Dim_Time.FirstOrDefault(t => t.date_Date == date);
            if (timeEntry == null)
            {
                timeEntry = new Dim_Time
                {
                    date_Date = date,
                    date_Year = date.Year,
                    date_Month = date.Month,
                    date_Quarter = (date.Month - 1) / 3 + 1,
                    Weekday_Name = date.DayOfWeek.ToString(),
                    Is_Weekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday
                };
                context.Dim_Time.Add(timeEntry);
                context.SaveChanges();
            }

            var historyEntry = new Fact_Market_Asset_History
            {
                Asset_ID = asset.ID,
                Time_ID = timeEntry.ID,
                Open_Price = (decimal?)entry.open,
                Close_Price = (decimal?)entry.close,
                Trading_Volume = (decimal?)entry.volumeto
            };

            context.Fact_Market_Asset_History.Add(historyEntry);
        }

        context.SaveChanges();
        Console.WriteLine("Fact_Market_Asset_History seeded successfully.");
    }
}