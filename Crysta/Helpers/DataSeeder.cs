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
            API_Source = $"https://api.coingecko.com/api/v3/coins/{bitcoin.id}"
        };

        context.Dim_Market_Asset.Add(newAsset);
        await context.SaveChangesAsync();

        Console.WriteLine("Seeded Bitcoin asset from CoinGecko.");
    }
}