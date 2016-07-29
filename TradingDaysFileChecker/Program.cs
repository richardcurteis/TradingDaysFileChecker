// Needs to reference QuanConnect main solution.
// In addition, a reference from QuanConnect.Algorithm.References.NodaTime will need to be added.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuantConnect;
using QuantConnect.Securities;
using QuantConnect.Securities.Forex;

namespace TradingDaysFileChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var startDate = new DateTime(2007, 04, 01);
            var endDate = new DateTime(2016, 07, 25);
            var dataFilePath = @"C:\Users\RichardsPC\Desktop\export\exporter\forex\fxcm\minute\";

            var securityType = SecurityType.Forex;
            var ticker = TickType.Trade;
            var marketHoursDatabase = MarketHoursDatabase.FromDataFolder();
            var market = Market.FXCM;
            var symbol = Symbol.Create(ticker.ToString(), securityType, market);
            var marketHoursDbEntry = marketHoursDatabase.GetEntry(symbol.ID.Market, symbol.Value, symbol.ID.SecurityType);

            var exchange = new ForexExchange(marketHoursDbEntry.ExchangeHours);
            var validTradingDays = IterateOverDateRange(exchange, startDate, endDate);

            var forexSecuritiesFolders = Directory.GetDirectories(dataFilePath);
            var missingDays = new List<Tuple<string, string>>();

            foreach (var validDay in validTradingDays)
            {
                foreach (var forexSecurity in forexSecuritiesFolders)
                {
                    var fxPair = new DirectoryInfo(forexSecurity).Name;
                    var formattedDate = FormatDate(validDay);
                    var path = SetPath(dataFilePath, fxPair, formattedDate);

                    if (File.Exists(path))
                        continue;
                    missingDays.Add(Tuple.Create(fxPair, formattedDate));
                }
            }

            if (missingDays.Count > 0)
                foreach (var missingDay in missingDays.OrderBy(md => md.Item1))
                    Console.WriteLine(missingDay.ToString().TrimEnd(')').TrimStart('('));
            else
                Console.WriteLine("No days missing");

            Console.WriteLine("Records missing: " + missingDays.Count);
            Console.ReadLine();
        }

        public static string FormattedFileName(string tradingDay)
        {
            return tradingDay + "_quote.zip";
        }

        public static string FormatDate(DateTime validDay)
        {
            return validDay.ToString("yyyyMMdd");
        }

        public static string SetPath(string dataFilePath, string fxPair, string formattedDate)
        {
            return dataFilePath + fxPair + @"\" + FormattedFileName(formattedDate);
        }

        public static IEnumerable<DateTime> IterateOverDateRange(ForexExchange exchange, DateTime start, DateTime end)
        {
            for (var day = start.Date; day.Date <= end.Date; day = day.AddDays(1))
                if (exchange.IsOpenDuringBar(day.Date, day.Date.AddDays(1), false))
                {
                    yield return day;
                }
        }
    }
}