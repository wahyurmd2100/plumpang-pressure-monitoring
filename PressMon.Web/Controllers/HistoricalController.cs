using Microsoft.AspNetCore.Mvc;
using PressMon.Web.Data;
using System.Linq;
using System;
using System.Linq.Dynamic.Core;

namespace TMS.Web.Controllers
{
    public class HistoricalController : Controller
    {
        private readonly DataContext _context;
        public HistoricalController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        //load data to datatable
        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();// Skip number of Rows count  
                var length = Request.Form["length"].FirstOrDefault(); // Paging Length 10,20  
                var searchValue = Request.Form["search[value]"].FirstOrDefault(); // Search Value from (Search box)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0; //Paging Size (10, 20, 50,100)  
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                // Get all data from the database
                var historicals = _context.Historicals.ToList().Select(p => new
                {
                    p.HistoricalID,
                    p.LocationName,
                    p.Pressure,
                    TimeStamp = UnixTimeStampToDateTime(p.TimeStamp)
                }).OrderBy(t => t.HistoricalID).Reverse();

                // Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    historicals = historicals.Where(m => m.TimeStamp.Contains(searchValue));
                }

                // Total number of rows counts   
                recordsTotal = historicals.Count();
                //Paging   
                var data = historicals.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static string UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Convert Unix timestamp to DateTimeOffset            
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)unixTimeStamp);

            // Get the Jakarta time zone
            TimeZoneInfo jakartaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");

            // Convert the DateTimeOffset to Jakarta time
            DateTimeOffset jakartaDateTimeOffset = TimeZoneInfo.ConvertTime(dateTimeOffset, jakartaTimeZone);

            // Convert the DateTimeOffset to the desired datetime string format
            string datetimeString = jakartaDateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss");

            return datetimeString;
        }

        private static int DateTimeToUnixTimeStamp(string dateTimeString)
        {
            // Parse the datetime string
            DateTime dateTime = DateTime.Parse(dateTimeString);

            // Get the Jakarta time zone
            TimeZoneInfo jakartaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");

            // Convert DateTime to DateTimeOffset in Jakarta time
            DateTimeOffset jakartaDateTimeOffset = new DateTimeOffset(dateTime, jakartaTimeZone.GetUtcOffset(dateTime));

            // Convert Jakarta DateTimeOffset to Unix timestamp
            int unixTimeStamp = (int)jakartaDateTimeOffset.ToUnixTimeSeconds();

            return unixTimeStamp;
        }

    }
}
