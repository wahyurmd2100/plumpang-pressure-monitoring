using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PressMon.Web.Data;
using PressMon.Web.Models;
using PressMon.Web;
using PressMon.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace PressMon.Web.Controllers
{
    public class TankHistoricalController : Controller
    {
        private readonly TMSContext _context;
        public TankHistoricalController(TMSContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            populateTanks();
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
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault(); // Sort Column Name  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();// Sort Column Direction (asc, desc)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault(); // Search Value from (Search box)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0; //Paging Size (10, 20, 50,100)  
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                //custom filter
                var TankId = Convert.ToInt32(Request.Form["TankNameFilter"].FirstOrDefault());//Tank Id
                var Datefrom = DateTime.Parse(Request.Form["DateFrom"].FirstOrDefault());//Operation Type
                var DateTo = DateTime.Parse(Request.Form["DateTo"].FirstOrDefault()).AddDays(1).AddSeconds(-1);//Operation Type

                //get all data
                var tankhistorical = (from p in _context.Tank
                                     join t in _context.Tank_Historical on p.TankId equals t.TankId
                                     join c in _context.Master_Products on p.ProductId equals c.ProductId
                                     select new
                                     {
                                         t.TankId,
                                         p.Name,
                                         c.ProductName,
                                         t.TimeStamp,
                                         t.LiquidLevel,
                                         t.WaterLevel,
                                         t.LiquidTemperature,
                                         t.LiquidDensity,
                                         t.VolumeObserved,
                                         t.VolumeNetStandard
                                     }).Where(t => t.TimeStamp >= Datefrom && t.TimeStamp <= DateTo);
                //filter tank name
                if (TankId != 0)
                {
                    tankhistorical = tankhistorical.Where(t => t.TankId == TankId);
                }
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    tankhistorical = tankhistorical.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tankhistorical = tankhistorical.Where(m => m.Name.Contains(searchValue));
                }

                //total number of rows counts   
                recordsTotal = tankhistorical.Count();
                //Paging   
                var data = tankhistorical.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void populateTanks(object SelectList = null)
        {
            List<Tank> tanks = new List<Tank>();
            tanks = (from t in _context.Tank select t).ToList();
            var tankip = new Tank()
            {
                TankId = 0,
                Name = "---- Select Tank ----"
            };
            tanks.Insert(0, tankip);
            ViewBag.TankId = tanks;
        }
    }
}
