using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using VeggieMarketDataStore.DbInterfaces;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbServices
{
    public class MetadataDbService : IMetadataDbService
    {
        private readonly DbService dbService;
        private readonly MarketDbService marketDbService;

        public MetadataDbService(DbService dbService, MarketDbService marketDbService)
        {
            this.dbService = dbService;
            this.marketDbService = marketDbService;
        }

        public MarketAvailableData[] GetAvailablePrices()
        {
            Market[] markets = marketDbService.GetMarkets();
            MarketAvailableData[] availablePrices = new MarketAvailableData[markets.Length];
            for (int i = 0; i < availablePrices.Length; i++)
            {
                availablePrices[i] = new MarketAvailableData(markets[i], GetMarketAvailablePrices(markets[i]));
            }

            return availablePrices;
        }

        public DatePeriod[] GetMarketAvailablePrices(Market market)
        {
            string query = "SELECT DISTINCT Date FROM " + DbService.PROCESSED_PRODUCT_PRICES_TABLE + " WHERE Market=" + market.MarketId;
            DbConnection connection = dbService.OpenConnection();
            DbDataReader reader = dbService.Select(connection, query);
            List<long> dbDates = new List<long>();
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    dbDates.Add(Convert.ToInt64(reader.GetValue(reader.GetOrdinal("Date"))));
                }
            }
            dbService.CloseConnection(connection, reader);

            List<DateTime> dates = new List<DateTime>();
            foreach (long dbDate in dbDates)
            {
                dates.Add(new DateTime(dbDate));
            }
            DatePeriod[] datePeriods = GroupDatesToPeriods(dates);

            return datePeriods;
        }

        private DatePeriod[] GroupDatesToPeriods(List<DateTime> dates)
        {
            if (dates  == null || dates.Count == 0) return new DatePeriod[0];

            dates.Sort();
            var groups = new List<List<DateTime>>();
            var group1 = new List<DateTime>() { dates[0] };
            groups.Add(group1);

            DateTime lastDate = dates[0];
            for (int i = 1; i < dates.Count; i++)
            {
                DateTime currDate = dates[i];
                TimeSpan timeDiff = currDate - lastDate;
                //should we create a new group?
                bool isNewGroup = timeDiff.Days > 1;
                if (isNewGroup)
                {
                    groups.Add(new List<DateTime>());
                }
                groups.Last().Add(currDate);
                lastDate = currDate;
            }

            List<DatePeriod> datePeriods = new List<DatePeriod>();
            foreach (List<DateTime> group in groups)
            {
                if (group.Count == 1)
                {
                    datePeriods.Add(new DatePeriod(group[0]));
                }
                else
                {
                    DateTime startDate = group[0];
                    DateTime endDate = group[group.Count - 1];
                    datePeriods.Add(new DatePeriod(startDate, endDate));
                }
            }

            return datePeriods.ToArray();
        }
    }
}
