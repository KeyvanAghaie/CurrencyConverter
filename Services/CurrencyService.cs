using CurrencyConverter.Models;
using CurrencyConverter.Models.Eurofxref;
using System.Globalization;
using System.Xml.Serialization;

namespace CurrencyConverter.Services
{
    public class CurrencyService : ICurrencyService
    {
        private List<Currency> currencies { get; set; }
        private DateTime lastUpdate { get; set; }

        public async Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency , decimal amount)
        {
            var fromRate = await GetLatestRate(fromCurrency);
            var toRate = await GetLatestRate(toCurrency);
            return amount / fromRate * toRate;
        }

        public async Task UpdateConfiguration()
        {
            //TODO Set CacheDurationInMinutes in appsettings.json
            var isCacheExpired = DateTime.Now.Subtract(lastUpdate) > TimeSpan.FromMinutes(5);
            if (currencies != null && isCacheExpired == false)
                return;

            var rawCurrencyList = await GetXmlFromSource();
            var envelope = await ParseHttpResponseToEnvelope(rawCurrencyList);
            currencies = ParseEnvelopeToCurrency(envelope);
            lastUpdate = DateTime.Now;
        }

        #region Private Methods
        private async Task<decimal> GetLatestRate(string currency)
        {
            if (currency.ToLower() == "eur")
                return 1;

            await UpdateConfiguration();
            var rate = currencies.Single(x => x.Name.ToLower() == currency.ToLower()).Rates.OrderByDescending(x => x.Timestamp).First().Rate;
            return rate;
        }

        private async Task<HttpResponseMessage> GetXmlFromSource()
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                //TODO: move this url to appsettings.json
                var path = "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml";
                response = await client.GetAsync(path);
            }
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception("download xml error");
            }
            return response;
        }

        private async Task<Envelope> ParseHttpResponseToEnvelope(HttpResponseMessage rawCurrencyList)
        {
            var stringCurrencyList = await rawCurrencyList.Content.ReadAsStringAsync();
            var serializer = new XmlSerializer(typeof(Envelope));
            Envelope result;
            using (TextReader reader = new StringReader(stringCurrencyList))
            {
                result = (Envelope)serializer.Deserialize(reader);
            }
            return result;
        }

        private List<Currency> ParseEnvelopeToCurrency(Envelope envelope)
        {
            var currencyResult = new List<Currency>();
            foreach (var TimestampWithList in envelope.Cube.CubeDates)
            {
                var timestamp = Convert.ToDateTime(TimestampWithList.Time);
                foreach (var RateWithCurrency in TimestampWithList.Cube)
                {
                    var name = RateWithCurrency.Currency;
                    var rate = Convert.ToDecimal(RateWithCurrency.Rate, new CultureInfo("en-US"));

                    var currentCurency = currencyResult.SingleOrDefault(x => x.Name == name);
                    if (currentCurency == null)
                    {
                        currentCurency = new Currency { Name = name, Rates = new List<CurrencyRate>() };
                        currencyResult.Add(currentCurency);
                    }
                    currentCurency.Rates.Add(new CurrencyRate() { Rate = rate, Timestamp = timestamp });
                }
            }
            return currencyResult;
        }

        #endregion

    }
}
