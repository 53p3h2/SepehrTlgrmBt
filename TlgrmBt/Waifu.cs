using Newtonsoft.Json;
using static Bot.Crypto.Crypto;

namespace Bot.Waifu
{
    internal class Waifu
    {
        public async static Task<string> GetWaifuURL()
        {
            HttpClient httpClient = new();
            string API = "https://api.waifu.pics/sfw/waifu";
            HttpResponseMessage response = await httpClient.GetAsync(API);
            string apiResponse = await response.Content.ReadAsStringAsync();

            ApiResponseWrapper apiWrapper = JsonConvert.DeserializeObject<ApiResponseWrapper>(apiResponse);
            string url = apiWrapper.url;
            return url;
        }
        public class ApiResponseWrapper
        {
            public string url { get; set; }
        }
    }
}
