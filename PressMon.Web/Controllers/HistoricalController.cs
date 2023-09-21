using Microsoft.AspNetCore.Mvc;
using PressMon.Web.Data;
using System.Linq;
using System;
using System.Linq.Dynamic.Core;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // Import the XSSF namespace
using NPOI.HSSF.Record.Chart;
using Microsoft.Extensions.Logging;
using PressMon.Web.Controllers;

namespace TMS.Web.Controllers
{
    public class HistoricalController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        public HistoricalController(ILogger<HistoricalController> logger,DataContext context)
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

                int unixDateFrom =(int) DateFrom.ToUnixTimeSeconds();
                int unixDateTo = (int)DateUntil.ToUnixTimeSeconds();
                // Get all data from the database
                //var historicals = _context.Historicals.ToList().Select(p => new
                //{
                //    p.HistoricalID,
                //    p.LocationName,
                //    p.Pressure,
                //    TimeStamp = UnixTimeStampToDateTime(p.TimeStamp)
                //}).OrderBy(t => t.HistoricalID).Reverse();
                var result= (from p in _context.Historicals
                                   select p).OrderBy(t => t.HistoricalID).Reverse().Where(t=>t.TimeStamp>=unixDateFrom && t.TimeStamp<=unixDateTo).ToList();
               

                var historicals = from p in result select new
                {
                    p.HistoricalID,
                    p.LocationName,
                    p.Pressure,
                    TimeStamp = UnixTimeStampToDateTime(p.TimeStamp)
                };

                // Total number of rows counts   
                recordsTotal = historicals == null? 0:result.Count();
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

        //Distribution Planning Export To Excel 
        [HttpPost]
        public IActionResult ExportToExcel(DateTime? DateFrom, DateTime? DateUntil)
        {
            try
            {
                // Open the template file
                FileStream templateStream = new FileStream("wwwroot/assets/docTemplate/TemplateHistorical.xlsx", FileMode.Open, FileAccess.Read);

                // Create a new workbook object based on the template file
                XSSFWorkbook workbook = new XSSFWorkbook(templateStream);

                // Get the sheet you want to write data to
                ISheet sheet = workbook.GetSheet("Historical");

                // Set Date Export
                IRow rowDate = sheet.CreateRow(5);
                rowDate.CreateCell(1).SetCellValue("Date Export : ");
                rowDate.CreateCell(2).SetCellValue(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                // Get the data model from your database or other source
                var historicals = _context.Historicals.ToList().Select(p => new
                {
                    p.HistoricalID,
                    p.LocationName,
                    p.Pressure,
                    TimeStamp = UnixTimeStampToDateTime(p.TimeStamp)
                });

                // Filter data based on the selected date range
                if (DateFrom != null && DateUntil != null)
                {
                    historicals = historicals.Where(t => t.TimeStamp >= DateFrom && t.TimeStamp <= DateUntil);
                }
                else if (DateFrom == null && DateUntil != null)
                {
                    historicals = historicals.Where(t => t.TimeStamp <= DateUntil);
                }
                else if (DateFrom != null && DateUntil == null)
                {
                    historicals = historicals.Where(t => t.TimeStamp >= DateFrom);
                }

                // Write the data model to the cells in the sheet
                int rowIndex = 9;
                int number = 1;
                // Start at row 1 (0-based index) to skip the header row
                foreach (var h in historicals)
                {

                    IRow row = sheet.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellValue(number);
                    row.CreateCell(1).SetCellValue(h.LocationName);
                    row.CreateCell(2).SetCellValue(h.Pressure);
                    row.CreateCell(3).SetCellValue(h.TimeStamp.ToString());

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
            catch(Exception)
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
