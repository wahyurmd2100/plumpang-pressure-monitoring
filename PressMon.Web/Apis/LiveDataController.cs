using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using PressMon.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TMS.Web.Hubs;
using TMS.Web.Models;
using TMS.Web.Models.sendWA;
using static TMS.Web.Apis.LiveDataController;

namespace TMS.Web.Apis
{

    [ApiController]
    [Route("api/[controller]")]
    public class LiveDataController : ControllerBase
    {
        private readonly IHubContext<HomeHub> _hubContext;
        private readonly DataContext _context;
        //
        //private enum Alarm_val : int
        //{
        //    LL = 10,
        //    L = 20,
        //    H = 30,
        //    HH = 40
        //}
        public LiveDataController(IHubContext<HomeHub> hubContext, DataContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }
        /// <summary>
        /// PostData - DataPost
        /// </summary>
        /// <param name="dataPost"></param>
        /// <returns></returns>
        [Route("~/api/PostData")]
        [HttpPost]
        public async Task<IActionResult> PostData([FromBody] DataPost dataPost)
        {
            var data = _context.LiveDatas.FirstOrDefault(x => x.LocationName == dataPost.LocationName);
            if (data != null)
            {
                data.Pressure =Math.Round(dataPost.Pressure,2);
                data.TimeStamp = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeSeconds());
                _context.Update(data);
                //
                SetAlarm(data);
                //
                Historical historical = new Historical();
                historical.LocationName = data.LocationName;
                historical.Pressure = Math.Round(dataPost.Pressure, 2);
                historical.TimeStamp = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeSeconds());
                _context.Add(historical);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveData", data);//signal R PUBLISH
                return Ok(new { Success = true, Message = "Update Success" });
            }
            return Ok(new { Success = false, Message = "Update Failed" });
        }
        public async Task<IActionResult> getAlarmSettings()
        {
            var data = (from p in _context.AlarmSettings
                        select new
                        {
                            p.AlarmSettingID,
                            p.Value,
                            p.Info
                        }).ToList();

            if (data != null)
            {
                return Ok(new { Success = true, data = data });
            }
            return Ok(new { Success = false, Message = "Get AlarmSettings Failed" });
        }
        /// <summary>
        /// Set Alarm
        /// </summary>
        /// <param name="liveData"></param>
        private void SetAlarm(LiveData liveData)
        {
            double LLpoint = 0;
            double Lpoint = 0;
            double Hpoint = 0;
            double HHpoint = 0;

            var alarmPoint = (from p in _context.AlarmSettings
                              select new
                              {
                                  p.AlarmSettingID,
                                  p.Value
                              });

            foreach (var alarmPoints in alarmPoint)
            {
                if (alarmPoints.AlarmSettingID == 1)
                {
                    LLpoint = alarmPoints.Value;
                }else if (alarmPoints.AlarmSettingID == 2)
                {
                    Lpoint = alarmPoints.Value;
                }
                else if (alarmPoints.AlarmSettingID == 3)
                {
                    Hpoint = alarmPoints.Value;
                }
                else if (alarmPoints.AlarmSettingID == 4)
                {
                    HHpoint = alarmPoints.Value;
                }
            }

            Alarm alarm = null;
            if (liveData.Pressure <= Lpoint && liveData.Pressure > LLpoint)
            {
                alarm = new Alarm { AlarmStatus = "L", LocationName = liveData.LocationName, Pressure = liveData.Pressure, TimeStamp = liveData.TimeStamp };
                sendWA();
            }
            else if (liveData.Pressure >= Hpoint && liveData.Pressure < HHpoint)
            {
                alarm = new Alarm { AlarmStatus = "H", LocationName = liveData.LocationName, Pressure = liveData.Pressure, TimeStamp = liveData.TimeStamp };
            }
            else if (liveData.Pressure <= LLpoint)
            {
                alarm = new Alarm { AlarmStatus = "LL", LocationName = liveData.LocationName, Pressure = liveData.Pressure, TimeStamp = liveData.TimeStamp };
            }
            else if (liveData.Pressure >= HHpoint)
            {
                alarm = new Alarm { AlarmStatus = "HH", LocationName = liveData.LocationName, Pressure = liveData.Pressure, TimeStamp = liveData.TimeStamp };
            }

            if(alarm != null)
            {
                //WApostStatus(alarm);
                _context.Add(alarm);
            }
        }

        private async Task sendWA()
        {
            var apiUrl = "https://multichannel.qiscus.com/whatsapp/v1/uwqks-1zpihp7ouot8pyv/4394/messages";
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Qiscus-App-Id", "uwqks-1zpihp7ouot8pyv");
            httpClient.DefaultRequestHeaders.Add("Qiscus-Secret-Key", "407e74a091d93e91db35b5c8c1b0b708");
            var responseMsg = "";

            var pressPT01 = _context.LiveDatas.FirstOrDefault(n => n.LocationName == "M-01");
            var pressPT02 = _context.LiveDatas.FirstOrDefault(n => n.LocationName == "M-02");
            var timestamp = DateTime.Now.ToString("dd/MMM/yy HH:mm");
            var numWAList = _context.WaContactLists
                            .Select(c => c.ContactNumber)
                            .ToList();

            foreach (var numWA in numWAList)
            {
                // Prepare your data to be sent in the POST request
                var parameters = new List<Parameter>
                    {
                        new Parameter { type = "text", text = pressPT01.Pressure.ToString() }, // Replace "4.4" with your actual value
                        new Parameter { type = "text", text = pressPT02.Pressure.ToString() }, // Replace "4.8" with your actual value
                        new Parameter { type = "text", text = timestamp } // Replace with your actual date value
                    };
                var components = new List<Component>
                    {
                        new Component { type = "body", parameters = parameters }
                    };
                var language = new Language { policy = "deterministic", code = "id" };
                var template = new Template { @namespace = "6e85836a_6b63_48a8_9d2f_e8c8c365e157", name = "pressure_manual_send", language = language, components = components };
                var requestData = new RequestMessage() { to = numWA, type = "template", template = template };

                // Serialize the data to JSON
                var jsonPayload = JsonSerializer.Serialize(requestData);

                // Prepare the HTTP content with the JSON payload
                var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    responseMsg = $"Response : {responseContent}";
                }
                else
                {
                    responseMsg = $"Error : {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
        }

        private void WApostStatus(Alarm alarm)
        {
            WaAlarmMessageStatus WAstatus = null;
            WAstatus = new WaAlarmMessageStatus { status = 0, Alarm = alarm };
            WAstatus.Alarm = alarm;
            _context.Add(WAstatus);
        }
    }
    //
   
    public class DataPost
    {
        public string LocationName { get; set; }
        public double Pressure { get; set; }
    }

}
