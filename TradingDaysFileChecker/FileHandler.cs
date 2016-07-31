using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuantConnect.Securities.Forex;

namespace TradingDaysFileChecker
{
    public class FileHandler
    {

        private StreamWriter _writeToFile;
        private List<Tuple<string, string>> _missingDays;
        private string _dataFilePath;
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private ForexExchange _exchange;
        private IEnumerable<DateTime> _validTradingDays;
        private string[] _forexSecuritiesFolders;

        public FileHandler(ForexExchange exchange)
        {
            _startDate = new DateTime(2007, 04, 01);
            _endDate = new DateTime(2016, 07, 25);
            _exchange = exchange;

            _writeToFile = new StreamWriter(@"C:\Users\RichardsPC\Documents\MissingResults.txt");
            _dataFilePath = @"C:\Users\RichardsPC\Desktop\export\exporter\forex\fxcm\minute\";
            _forexSecuritiesFolders = Directory.GetDirectories(_dataFilePath);

            _missingDays = new List<Tuple<string, string>>();
            _validTradingDays = IterateOverDateRange(_exchange, _startDate, _endDate);
        }

        public void CheckForMissingFiles()
        {
            foreach (var validDay in _validTradingDays)
            {
                foreach (var forexSecurity in _forexSecuritiesFolders)
                {
                    var fxPair = new DirectoryInfo(forexSecurity).Name;
                    var formattedDate = FormatDate(validDay);
                    var path = SetPath(_dataFilePath, fxPair, formattedDate);

                    if (!File.Exists(path))
                    {
                        _missingDays.Add(Tuple.Create(fxPair, formattedDate));
                    }
                }
            }

        }

        public void GetResults()
        {
            _writeToFile.AutoFlush = true;
            _writeToFile.WriteLine($"Total missing files: {_missingDays.Count}");
            if (_missingDays.Count > 0)
            {
                foreach (var missingDay in _missingDays.OrderBy(md => md.Item1))
                {
                    var formattedTupleOutput = missingDay.ToString().TrimStart('(').TrimEnd(')');
                    Console.WriteLine(formattedTupleOutput);
                    WriteResultsToFile(formattedTupleOutput);
                }
            }
            else
            {
                var noFilesMissing = "No results missing";
                Console.WriteLine(noFilesMissing);
                WriteResultsToFile(noFilesMissing);
            }

            Console.WriteLine("Records missing: " + _missingDays.Count);
        }

        public void WriteResultsToFile(string result)
        {
            _writeToFile.WriteLine(result);
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