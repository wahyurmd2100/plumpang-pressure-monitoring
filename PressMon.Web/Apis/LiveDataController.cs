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
        private enum Alarm_val : int
        {
            LL = 10,
            L = 20,
            H = 30,
            HH = 40
        }
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
        /// <summary>
        /// Set Alarm
        /// </summary>
        /// <param name="liveData"></param>
        private void SetAlarm(LiveData liveData)
        {
            Alarm alarm = null;
            //
            if(liveData.Pressure <= (double)Alarm_val.LL)
            {
                alarm = new Alarm { AlarmStatus = "LL", LocationName = liveData.LocationName, Pressure = liveData.Pressure, TimeStamp = liveData.TimeStamp };
            }
            else if (liveData.Pressure > (double)Alarm_val.LL && liveData.Pressure <=(double)Alarm_val.L)
            {
                alarm = new Alarm { AlarmStatus = "L", LocationName = liveData.LocationName, Pressure = liveData.Pressure, TimeStamp = liveData.TimeStamp };
            }
            else if (liveData.Pressure > (double)Alarm_val.H && liveData.Pressure <= (double)Alarm_val.HH)
            {
                alarm = new Alarm { AlarmStatus = "H", LocationName = liveData.LocationName, Pressure = liveData.Pressure, TimeStamp = liveData.TimeStamp };
            }
            else if (liveData.Pressure > (double)Alarm_val.HH)
            {
                alarm = new Alarm { AlarmStatus = "HH", LocationName = liveData.LocationName, Pressure = liveData.Pressure, TimeStamp = liveData.TimeStamp };
            }
            if(alarm != null)
            {
                _context.Add(alarm);
            }
        }
    }
    //
   
    public class DataPost
    {
        public string LocationName { get; set; }
        public double Pressure { get; set; }
    }

}
