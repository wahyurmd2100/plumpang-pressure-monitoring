using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CSL.Web.Data;
using CSL.Web.Models;
using CSL.Web;
using Microsoft.AspNetCore.Authorization;
using PressMon.Web.Models;

namespace PressMon.Web.Controllers
{
    public class TankLiveDatasController : Controller
    {
        private readonly TMSContext _context;
        public TankLiveDatasController(TMSContext context)
        {
            _context = context;
        }
        public IActionResult Index()
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
                //get all data
                var tankLiveDatas = (from p in _context.Tank
                            join t in _context.Tank_Live_Data on p.TankId equals t.TankId
                            join c in _context.Master_Products on p.ProductId equals c.ProductId
                            select new
                            {
                                t.TankId,
                                p.Name,
                                c.ProductName,
                                t.TimeStamp,
                                t.Level,
                                t.Temperature,
                                t.GrossVolume,
                                t.NetVolume,
                                t.Density,
                                t.VCF,
                                t.LiquidWeight
                            });
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    tankLiveDatas = tankLiveDatas.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tankLiveDatas = tankLiveDatas.Where(m => m.Name.Contains(searchValue));
                }

                //total number of rows counts   
                recordsTotal = tankLiveDatas.Count();
                System.Diagnostics.Debug.WriteLine(recordsTotal);
                //Paging   
                var data = tankLiveDatas.Skip(skip).Take(pageSize).ToList();

                //System.Diagnostics.Debug.WriteLine(data);
                //Returning Json Data  
                //return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = 13 });
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
