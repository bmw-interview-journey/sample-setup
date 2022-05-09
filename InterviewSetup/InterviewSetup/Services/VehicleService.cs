using InterviewSetup.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace InterviewSetup.Services
{
    public class VehicleService
    {
        private const string Url = "https://vpic.nhtsa.dot.gov/api/vehicles/";
        private readonly HttpClient _client;

        public VehicleService()
        {
            _client = new HttpClient {BaseAddress = new Uri(Url)};
        }

        public async Task<IList<Wmi>> GetWMIsForManufacturer(string manufacturer)
        {
            var result = await _client.GetAsync($"GetManufacturerDetails/{manufacturer}?format=json");
            result.EnsureSuccessStatusCode();
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponse>(content);
            return response.Results;
        }
    }
}