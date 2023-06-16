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
            var data = _context.LiveDatas.FirstOrDefault(x => x.LocationName == "M-01").Pressure;
            return Json(data);
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

            var liveDatas = (from p in _context.Historicals select p).OrderBy(x => x).Reverse();
            var Datas = from p in liveDatas
                        select new
                        {
                            p.HistoricalID,
                            p.LocationName,
                            p.Pressure,
                            TimeStamp = UnixTimeStampToDateTime(p.TimeStamp)
                        };
            //total number of rows counts
            recordsTotal = Datas.Count();
            //paging
            var data = Datas.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFilterd = recordsTotal, recordsTotal = recordsTotal, data = data });

        }
        private static string UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Convert Unix timestamp to DateTimeOffset            
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)unixTimeStamp);
            // Convert DateTimeOffset to desired datetime string format
            string datetimeString = dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss");
            return datetimeString;
        }

    }
}
