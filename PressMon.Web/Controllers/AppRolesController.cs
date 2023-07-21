using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using SQLitePCL;
using PressMon.Web.Data;
using Microsoft.AspNetCore.Identity;
using PressMon.Web;
using System.Threading.Tasks;
using PressMon.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TMS.Web.Controllers
{
    public class AppRolesController : Controller
    {
        private readonly DataContext _context;
        public AppRolesController(DataContext context)
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

                // getting all data  
                var alarmSettings = from p in _context.AppRoles select p;

                recordsTotal = alarmSettings.Count();
                //Paging   
                var data = alarmSettings.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }

            catch (Exception)
            {
                throw;
            }
        }
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(string id = "")
        {
            if (id == "" || id == null)//flagged as insert
            {
                return View(new AppRole());
            }
            else
            {
                var role = await _context.AppRoles.FindAsync(id);
                return View(role);
            }
        }
        //POST: Section/AddOrEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string id, [Bind("Id, Name")] AppRole appRole)
        {
            try
            {
                var RoleFinder = await _context.AppRoles.FirstOrDefaultAsync(x => x.Name == appRole.Name && x.Id == id);
                if (RoleFinder == null)
                {

                    _context.Add(appRole);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Update(appRole);
                    await _context.SaveChangesAsync();
                }
                return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.AppRoles.ToList()) });
            }
            catch
            {
                return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", appRole) });
            }
        }

        private bool RoleExists(string id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
