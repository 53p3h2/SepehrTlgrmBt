using Newtonsoft.Json;

namespace Bot.NumTrivia
{
    internal class NumTrivia
    {
        public async static Task<string> GetNumTrivia(decimal num)
        {
            HttpClient httpClient = new();
            string API = "http://numbersapi.com/" + num;
            HttpResponseMessage response = await httpClient.GetAsync(API);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
