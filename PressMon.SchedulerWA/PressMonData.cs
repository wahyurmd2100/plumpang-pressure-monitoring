using PressMon.SchedulerWA.WaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PressMon.SchedulerWA
{
    internal class PressMonData
    {
        private readonly HttpClient _httpClient;

        public PressMonData()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5004/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<PressMonResponse> GetApiDataAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/schedulerData");

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    PressMonResponse pressMonResponse = JsonSerializer.Deserialize<PressMonResponse>(responseContent);
                    return pressMonResponse;
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve data from the API. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return null;
        }

    }
}
