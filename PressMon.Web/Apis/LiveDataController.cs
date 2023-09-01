using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using PressMon.Web.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMS.Web.Hubs;
using TMS.Web.Models;
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
            //WaAlarmMessageStatus WAstatus = null;
            //
            if (liveData.Pressure <= Lpoint && liveData.Pressure > LLpoint)
            {
                alarm = new Alarm { AlarmStatus = "L", LocationName = liveData.LocationName, Pressure = liveData.Pressure, TimeStamp = liveData.TimeStamp };
                //WAstatus = new WaAlarmMessageStatus { status = 1 };
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
                _context.Add(alarm);
            }

            //if (alarm != null && WAstatus != null)
            //{
            //    _context.Add(WAstatus);
            //}
        }

        //private void WApostStatus()
        //{
        //    WaAlarmMessageStatus waMessageStatus = new WaAlarmMessageStatus();
        //}
    }
    //
   
    public class DataPost
    {
        public string LocationName { get; set; }
        public double Pressure { get; set; }
    }

}
