using System;
using System.Threading.Tasks;
using BitcoinConverter.Code;

namespace BitcoinConverter.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var converter = new ConverterSvc();

            while(true)
            {
                try
                {
                    Console.Write("Coins: ");
                    var coins = Double.Parse(Console.ReadLine());
                    Console.Write("Currency (USD, GBP, or EUR");
                    var currency = (ConverterSvc.Currency)Enum.Parse(typeof(ConverterSvc.Currency), Console.ReadLine(), true);

                    var amount = await converter.BitcoinConverter(currency, coins);

                    Console.WriteLine($"{coins} Bitcoins = {amount} {currency}");

                }
                catch
                {
                   // swallow
                }
            }
        }
    }
}
