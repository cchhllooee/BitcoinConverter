using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

namespace BitcoinConverter.Code
{
    public class ConverterSvc
    {
        private const string BITCOIN_CURRENTPRICE_URL = "https://api.coindesk.com/v1/bpi/currentprice.json";
        private HttpClient client;

        public ConverterSvc()
        {
            this.client = new HttpClient();
        }
        public ConverterSvc(HttpClient httpClient)
        {
            this.client = httpClient;
        }

        public enum Currency
        {
            USD,
            GBP,
            EUR
        }

        public async Task<double> GetExchangeRate(Currency currency)
        {
            double rate = 0;

            try
            {
                var response = await this.client.GetStringAsync(BITCOIN_CURRENTPRICE_URL);

                var jsonDoc = JsonDocument.Parse(Encoding.ASCII.GetBytes(response));
                //get the rate from the bpi in the chose currency
                var rateStr = jsonDoc.RootElement.GetProperty("bpi").GetProperty(currency.ToString()).GetProperty("rate");

                rate = Double.Parse(rateStr.GetString());
            }
            catch
            {
                rate = -1;
            }

            return Math.Round(rate, 4);
        }

        public async Task<double> ConvertBitcoin(double coins, Currency currency)
        {
            double dollars = 0;

            if(coins < 0)
            {
                throw new ArgumentException("Number of coins should be positive");
            }

            var exchangeRate = await GetExchangeRate(currency);

            if(exchangeRate >= 0)
            {
                dollars = exchangeRate * coins;
            }
            else
            {
                dollars = -1;
            }

            return Math.Round(dollars,4);
        }
    }
}
