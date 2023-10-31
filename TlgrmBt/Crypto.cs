using Newtonsoft.Json;

namespace Bot.Crypto
{
    internal class Crypto
    {
        public async static Task<decimal> CryptoPrice(string cryptoSymbol)
        {
            HttpClient httpClient = new();
            const string API = "https://api.wallex.ir/v1/currencies/stats";
            HttpResponseMessage response = await httpClient.GetAsync(API);
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                ApiResponseWrapper apiWrapper = JsonConvert.DeserializeObject<ApiResponseWrapper>(apiResponse);
                List<DataItem> dataItems = apiWrapper.result;
                if (dataItems.Find(x => x.key == cryptoSymbol) == null)
                {
                    return 0;
                }
                else
                {
                    return (decimal)dataItems.Find(x => x.key == cryptoSymbol).price;
                }
            }
            else
            {
                return 0;
            }
        }

        public class ApiResponseWrapper
        {
            public List<DataItem> result { get; set; }
        }

        public class DataItem
        {
            public string? name_en { get; set; }
            public string? key { get; set; }
            public decimal? market_cap { get; set; }
            public decimal? price { get; set; }
            public decimal? percent_change_30d { get; set; }
            public decimal? total_supply { get; set; }
        }
    }
}
