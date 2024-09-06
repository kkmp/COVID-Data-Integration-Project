using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Projekt.Json
{
    public static class JsonData
    {
        private static readonly string dataUrl = "https://api.covid19api.com/summary";
        private static readonly string totalDataUrl = "https://api.covid19api.com/total/dayone/country";
        public static async Task<List<DailyCaseJsonModel>> GetDailyCases(HttpClient client)
        {
            var response = await client.GetAsync(dataUrl);
            var dataStr = await response.Content.ReadAsStringAsync();
            var data = (JObject)JsonConvert.DeserializeObject(dataStr); //Deserialziacja JSON
            return JsonConvert.DeserializeObject<List<DailyCaseJsonModel>>(data["Countries"].ToString());
        }

        public static async Task<List<ToalDailyCaseJsonModel>> GetTotalDailyCasesByCountryAsync(HttpClient client, string countryName)
        {
            var response = await client.GetAsync(totalDataUrl + "/" + countryName);
            var dataStr = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ToalDailyCaseJsonModel>>(dataStr);
        }
    }
}
