using PressMon.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Linq.Dynamic.Core;
using PressMon.Web.Data;
using PressMon.Web.Models;
using System.Collections.Generic;
using System;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.InkML;

namespace PressMon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetLiveData()
        {
            var data = _context.LiveDatas.ToList();
            var charHistory = (from p in _context.Historicals select p).OrderByDescending(t => t.HistoricalID).Take(40).OrderBy(t => t.TimeStamp).ToList();
            var result = from c in charHistory
                         select new
                         {
                             Pressure = c.Pressure,
                             LocationName = c.LocationName,
                             TimeStamp = UnixTimeStampToDateTime(c.TimeStamp)
                         };


            return Json(new { liveData = data, HistoryData = result });
        }


        public IActionResult GetAlarm()
        {
            var data = _context.LiveDatas.ToList();
            var alarms = _context.AlarmSettings.ToList();
            return Json(new { value = data, alarms = alarms });
        }
        public IActionResult GetDataRecord()
        {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();//Skip number of rows count
            var length = Request.Form["length"].FirstOrDefault();//paging length 10, 20
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();// sort column name
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();//sort column direction (asc, desc)
            var searchValue = Request.Form["search[value]"].FirstOrDefault();//search value from (search box)
            int pageSize = length != null ? Convert.ToInt32(length) : 0;//paging size (10, 20, 50, 100)
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            string timeInterval = Request.Form["columns[0][search][value]"].FirstOrDefault();

            var liveDatas = (from p in _context.Historicals select p).OrderBy(x => x).Reverse();
            var Datas = from p in liveDatas
                        select new
                        {
                            p.HistoricalID,
                            p.LocationName,
                            p.Pressure,
                            TimeStamp = UnixTimeStampToDateTime(p.TimeStamp)
                        };


            if (timeInterval == "Minutes")
            {
                // Group data by time interval
                var groupedData = Datas.GroupBy(model => new { model.TimeStamp.Year, model.TimeStamp.Month, model.TimeStamp.Day, model.TimeStamp.Hour, model.TimeStamp.Minute })
                    .Select(group => new
                    {
                        Minute = new DateTime(group.Key.Year, group.Key.Month, group.Key.Day, group.Key.Hour, group.Key.Minute, 0),
                        Models = group.ToList()
                    })
                    .ToList();

                // Flatten the grouped data and convert to IQueryable
                Datas = groupedData.SelectMany(group => group.Models).AsQueryable();
            }


            //total number of rows counts
            recordsTotal = Datas.Count();
            //paging
            var data = Datas.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFilterd = recordsTotal, recordsTotal = recordsTotal, data = data });

        }
        private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Convert Unix timestamp to DateTimeOffset            
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)unixTimeStamp);

            // Get the Jakarta time zone
            TimeZoneInfo jakartaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");

            // Convert the DateTimeOffset to Jakarta time
            DateTimeOffset jakartaDateTimeOffset = TimeZoneInfo.ConvertTime(dateTimeOffset, jakartaTimeZone);

            // Convert the DateTimeOffset to the desired datetime string format
            //string datetimeString = jakartaDateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss");

            return jakartaDateTimeOffset.DateTime;
        }
    }
}
