using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TMS.Web.Models.sendWA;

namespace PressMon.SchedulerWA
{
    internal class WhatsAppSender
    {
        private readonly HttpClient _httpClient;
        private string 
            apiUrl = "https://multichannel.qiscus.com/whatsapp/v1/uwqks-1zpihp7ouot8pyv/4394/messages",
            appId = "uwqks-1zpihp7ouot8pyv",
            secretKey = "407e74a091d93e91db35b5c8c1b0b708",
            languageCode = "id",
            templateNamespace = "6e85836a_6b63_48a8_9d2f_e8c8c365e157",
            templateName = "pressure_manual_send",
            requestMessageType = "template";
        
        public WhatsAppSender()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Qiscus-App-Id", appId);
            _httpClient.DefaultRequestHeaders.Add("Qiscus-Secret-Key", secretKey);
        }

        public async Task<RequestMessage> sendWA(string numWA, string pressPT01, string pressPT02)
        {
            var timestamp = DateTime.Now.ToString("dd/MMM/yy HH:mm");
            var parameters = new List<Parameter>
            {
                new Parameter { type = "text", text = pressPT01 },
                new Parameter { type = "text", text = pressPT02 },
                new Parameter { type = "text", text = timestamp }
            };

            var components = new List<Component>
            {
                new Component { type = "body", parameters = parameters }
            };

            var language = new Language { policy = "deterministic", code = languageCode };
            var template = new Template { @namespace = templateNamespace, name = templateName, language = language, components = components };
            var requestData = new RequestMessage() { to = numWA, type = requestMessageType, template = template };

            var jsonPayload = JsonSerializer.Serialize(requestData);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<RequestMessage>(responseContent);
                    return responseObject;
                }
                else
                {
                    Console.WriteLine($"Error sending data to WhatsApp API: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending data to WhatsApp API: {ex.Message}");
            }

            return null;

        }
    }
}
