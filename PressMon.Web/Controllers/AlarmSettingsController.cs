using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PressMon.Web;
using PressMon.Web.Data;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
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
        // GET: Banks/AddOrEdit
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)//flagged as insert
            {
                return View(new AlarmSettings());
            }
            else
            {
                var alarmSettings = await _context.AlarmSettings.FindAsync(id);
                if (alarmSettings == null)
                {
                    return NotFound();
                }
                return View(alarmSettings);
            }
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("AlarmSettingID,Value,Info")] AlarmSettings alarmSettings)
        {

            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    //alarmSettings.CreateTime = DateTime.Now;
                    //_context.Add(alarmSettings);
                    //await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        //alarmSettings.UpdateTime = DateTime.Now;
                        _context.Update(alarmSettings);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AlarmSettingsExists(alarmSettings.AlarmSettingID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.AlarmSettings.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", alarmSettings) });
        }

        private bool AlarmSettingsExists(int id)
        {
            return _context.AlarmSettings.Any(e => e.AlarmSettingID == id);
        }
    }
}
