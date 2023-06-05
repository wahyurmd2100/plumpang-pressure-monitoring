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

namespace TMS.Web.Apis
{
    [ApiController]
    [Route("api/[controller]")]    
    public class LiveDataController : ControllerBase
    {
        private readonly IHubContext<HomeHub> _hubContext;
        private readonly DataContext _context;
        public LiveDataController(IHubContext<HomeHub> hubContext, DataContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }
        [Route("~/api/PostData")]
        [HttpPost]
        public async Task<IActionResult> PostData([FromBody] DataPost dataPost)
        {
            var data = _context.LiveDatas.FirstOrDefault(x => x.LocationName == dataPost.LocationName);
            if (data != null)
            {
                data.Pressure = dataPost.Pressure;
                data.TimeStamp = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeSeconds());
                _context.Update(data);
                //
                Historical historical = new Historical();
                historical.LocationName = data.LocationName;
                historical.Pressure = data.Pressure;
                historical.TimeStamp = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeSeconds());
                _context.Add(historical);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveData", data);
                return Ok(new { Success = true, Message = "Update Success" });
            }
            return Ok(new { Success = false, Message = "Update Failed" });
        }
    }
    public class DataPost
    {
        public string LocationName { get; set; }
        public double Pressure { get; set; }
    }
}
