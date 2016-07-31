using System;
using QuantConnect;
using QuantConnect.Securities;
using QuantConnect.Securities.Forex;

namespace TradingDaysFileChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var securityType = SecurityType.Forex;
            var ticker = TickType.Trade;
            var marketHoursDatabase = MarketHoursDatabase.FromDataFolder();
            const string market = Market.FXCM;
            var symbol = Symbol.Create(ticker.ToString(), securityType, market);
            var marketHoursDbEntry = marketHoursDatabase.GetEntry(symbol.ID.Market, symbol.Value, symbol.ID.SecurityType);
            var exchange = new ForexExchange(marketHoursDbEntry.ExchangeHours);


            var checkFiles = new FileHandler(exchange);
            checkFiles.CheckForMissingFiles();

            Console.ReadLine();
        }
    }
}
