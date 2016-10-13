using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Ticker.Bot.Services
{
    public static class StockClient
    {
        public static async Task<double?> GetStockPriceAsync(string symbol)
        {
            // Check if the symbol is not empty
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return null;
            }
            else
            {
                // Query the symbol and the last sale
                // Documentation: http://www.jarloo.com/yahoo_finance/
                WebClient client = new WebClient();
                string url = $"http://finance.yahoo.com/d/quotes.csv?s={symbol}&f=sl1";
                string csv = await client.DownloadStringTaskAsync(url).ConfigureAwait(false);

                // Process the CSV result
                string line = csv.Split('\n')[0];
                string priceAsText = line.Split(',')[1];

                // Format the last sale as number
                NumberStyles style = NumberStyles.Any;
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
                double price;
                if (double.TryParse(priceAsText, style, culture, out price))
                {
                    return price;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}