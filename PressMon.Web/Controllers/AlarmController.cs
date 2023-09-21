using Microsoft.AspNetCore.Mvc;
using PressMon.Web.Data;
using System.Linq;
using System;
using TMS.Web.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace TMS.Web.Controllers
{
    public class AlarmController : Controller
    {
        private readonly DataContext _context;

        public AlarmController(DataContext context)
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

                DateTimeOffset DateFrom = DateTimeOffset.Parse(Request.Form["DateFrom"]);
                DateTimeOffset DateUntil = DateTimeOffset.Parse(Request.Form["DateUntil"]);

                int unixDateFrom = (int)DateFrom.ToUnixTimeSeconds();
                int unixDateUntil = (int)DateUntil.ToUnixTimeSeconds();

                var result = (from p in _context.Alarms
                              select p).OrderBy(t => t.AlarmID).Reverse().Where(t => t.TimeStamp >= unixDateFrom && t.TimeStamp <= unixDateUntil).ToList();

                //get all data
                var alarms = from p in result
                             select new
                             {
                                 p.AlarmID,
                                 p.AlarmStatus,
                                 p.LocationName,
                                 p.Pressure,
                                 TimeStamp = UnixTimeStampToDateTime(p.TimeStamp)
                             };

                //    _context.Alarms.ToList().Select(p => new
                //{
                //    p.AlarmID,
                //    p.AlarmStatus,
                //    p.LocationName,
                //    p.Pressure,
                //    TimeStamp = UnixTimeStampToDateTime(p.TimeStamp)
                //}).OrderBy(t => t.AlarmID).Reverse();

                //total number of rows counts   
                recordsTotal = alarms.Count();
                //Paging   
                var data = alarms.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Alarms Log Export To Excel 
        [HttpPost]
        public IActionResult ExportToExcel(DateTime? DateFrom, DateTime? DateUntil)
        {
            try
            {
                // Open the template file
                FileStream templateStream = new FileStream("wwwroot/assets/docTemplate/TemplateAlarmsLog.xlsx", FileMode.Open, FileAccess.Read);

                // Create a new workbook object based on the template file
                XSSFWorkbook workbook = new XSSFWorkbook(templateStream);

                // Get the sheet you want to write data to
                ISheet sheet = workbook.GetSheet("Historical");

                // Set Date Export
                IRow rowDate = sheet.CreateRow(5);
                rowDate.CreateCell(0).SetCellValue("Date Export : ");
                rowDate.CreateCell(1).SetCellValue(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                // Convert DateTime? to DateTimeOffset?
                DateTimeOffset? dateFromOffset = DateFrom.HasValue ? new DateTimeOffset(DateFrom.Value) : null;
                DateTimeOffset? dateUntilOffset = DateUntil.HasValue ? new DateTimeOffset(DateUntil.Value) : null;

                int unixDateFrom = dateFromOffset.HasValue ? (int)dateFromOffset.Value.ToUnixTimeSeconds() : 0;
                int unixDateUntil = dateUntilOffset.HasValue ? (int)dateUntilOffset.Value.ToUnixTimeSeconds() : (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var result = (from p in _context.Alarms
                              select p).OrderBy(t => t.AlarmID).Reverse().Where(t => t.TimeStamp >= unixDateFrom && t.TimeStamp <= unixDateUntil);

                // Get the data model from your database or other source
                var alarms = from p in result
                             select new
                             {
                                 p.AlarmID,
                                 p.AlarmStatus,
                                 p.LocationName,
                                 p.Pressure,
                                 TimeStamp = UnixTimeStampToDateTime(p.TimeStamp)
                             };

                // Write the data model to the cells in the sheet
                int rowIndex = 8;
                int number = 1;
                // Start at row 1 (0-based index) to skip the header row
                foreach (var a in alarms)
                {

                    IRow row = sheet.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellValue(number);
                    row.CreateCell(1).SetCellValue(a.AlarmStatus);
                    row.CreateCell(2).SetCellValue(a.LocationName);
                    row.CreateCell(3).SetCellValue(a.Pressure);
                    row.CreateCell(4).SetCellValue(a.TimeStamp.ToString());

                    rowIndex++;
                    number++;
                }

                // Save the workbook to a new file
                MemoryStream stream = new MemoryStream();
                workbook.Write(stream);
                stream.Position = 0;
                FileStreamResult file = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                file.FileDownloadName = "Pressure Transmitter Historical.xlsx";

                // Close the streams
                templateStream.Close();

                return file;
            }
            catch (Exception)
            {
                throw;
            }
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
