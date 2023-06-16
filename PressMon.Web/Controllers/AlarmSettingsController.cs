using Microsoft.AspNetCore.Mvc;
using PressMon.Web.Data;
using System.Linq;
using System;
using System.Linq.Dynamic.Core;
using TMS.Web.Models;

namespace TMS.Web.Controllers
{
    public class AlarmSettingsController : Controller
    {
        private readonly DataContext _context;

        public AlarmSettingsController(DataContext context)
        {
            _context = context;
        }

        // GET: AlarmSettingsController
        public ActionResult Index()
        {
            return View();
        }
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

                // getting all data  
                var product = (from p in _context.AlarmSettings select p);//LINQ
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    product = product.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    product = product.Where(m => m.Info.Contains(searchValue));
                }

                //total number of rows counts   
                recordsTotal = product.Count();
                //Paging   
                var data = product.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}
