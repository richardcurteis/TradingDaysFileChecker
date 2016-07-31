using System;
using System.Linq;
using QuantConnect;
using QuantConnect.Configuration;
using QuantConnect.Data.Market;
using QuantConnect.ToolBox;
using QuantConnect.ToolBox.FxcmDownloader;

namespace TradingDaysFileChecker
{
    public class Downloader
    {
        private readonly string _dataDirectory;
        private readonly string _server;
        private readonly string _terminal;
        private readonly string _userName;
        private readonly string _password;
        private FxcmDataDownloader _dataDownload;
        public Downloader()
        {
            // Load settings from config.json
            _dataDirectory = Config.Get("data-directory", @"C:\Data");
            _server = Config.Get("fxcm-server", "http://www.fxcorporate.com/Hosts.jsp");
            _terminal = Config.Get("fxcm-terminal", "Demo");
            _userName = Config.Get("fxcm-user-name", "D102538320001");
            _password = Config.Get("fxcm-password", "2807");
            _dataDownload = new FxcmDataDownloader(_server, _terminal, _userName, _password);
        }

        public void Download(Symbol symbol, Resolution resolution, DateTime startDate, DateTime endDate)
        {
            endDate = endDate.AddDays(1).AddMilliseconds(-1);
            var data = _dataDownload.Get(symbol, resolution, startDate, endDate);
            var ticks = data.Cast<Tick>().ToList();
            var resData = FxcmDataDownloader.AggregateTicks(symbol, ticks, Resolution.Minute.ToTimeSpan());
            var writer = new LeanDataWriter(resolution, symbol, _dataDirectory);
            writer.Write(resData);
        }
    }
}