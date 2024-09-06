using Client.Models;
using Newtonsoft.Json;
using Projekt.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ConnectionHandler
    {
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:8080/") };
        private static ConnectionHandler instance;
        public static ConnectionHandler Instance { 
            get 
            {  
                if (instance == null)
                {
                    instance = new ConnectionHandler();
                }
                return instance;
            } 
        }

        private ConnectionHandler()
        {

        }

        private class Response
        {
            public string? Message { get; set; }
            public string? Token { get; set; }
        }

        public async Task Register(string? username, string? password, Action<string?>? callback = null)
        {
            var response = await httpClient.PostAsJsonAsync("/api/Auth/register", new { username = username, password = password });
            var message = await response.Content.ReadFromJsonAsync<Response>();
            callback?.Invoke(message?.Message);
        }

        public async Task Login(string? username, string? password, Action<string?, bool>? callback = null)
        {
            var response = await httpClient.PostAsJsonAsync("/api/Auth", new { username = username, password = password });
            var message = await response.Content.ReadFromJsonAsync<Response>();
            bool success = false;
            if(message?.Token != null)
            {
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", $"Bearer {message.Token}");
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + message.Token);
                success = true;
            }
            callback?.Invoke(message?.Message, success);
        }

        public async Task<List<Country>> GetCountries()
        {
            var response = await httpClient.GetAsync("/api/Data/Countries");
            var list = await response.Content.ReadFromJsonAsync<List<Country>>();
            return list.OrderBy(x => x.Name).ToList();
        }

        public async Task<List<DailyCase>> GetResults(Guid countryId, DateTime startDate, DateTime stopDate)
        {
            var response = await httpClient.GetAsync($"/api/Data/Filter?countryId={countryId}&dateFrom={startDate.ToString("yyyy-MM-dd")}&dateTo={stopDate.ToString("yyyy-MM-dd")}");

            return await response.Content.ReadFromJsonAsync<List<DailyCase>>();
        }
    }
}
