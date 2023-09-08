using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using PressMon.Web.Data;
using PressMon.Web;
using System.Threading.Tasks;
using TMS.Web.Models;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;

namespace TMS.Web.Controllers
{
    public class WaContactListController : Controller
    {
        private readonly DataContext _context;
        public WaContactListController(DataContext context)
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
                var searchValue = Request.Form["search[value]"].FirstOrDefault(); // Search Value from (Search box)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0; //Paging Size (10, 20, 50,100)  
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                //get all data
                var contact = (from p in _context.WaContactLists
                              select new
                              {
                                  p.ContactID,
                                  p.ContactName,
                                  p.ContactNumber
                              }).OrderBy(t => t.ContactID).Reverse();

                //total number of rows counts   
                recordsTotal = contact.Count();
                //Paging   
                var data = contact.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new WaContactList());
            }
            else
            {
                var tank = await _context.WaContactLists.FindAsync(id);
                if (tank == null)
                {
                    return NotFound();
                }
                return View(tank);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("ContactID, ContactName, ContactNumber")] WaContactList waContactList
            )
        {

            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Add(waContactList);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    try
                    {
                        _context.Update(waContactList);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!contactExist(waContactList.ContactID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.WaContactLists.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", waContactList) });

        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.WaContactLists.FindAsync(id);
            _context.WaContactLists.Remove(contact);
            await _context.SaveChangesAsync();
            return Json(new { html = Helper.RenderRazorViewString(this, "_ViewAll", _context.WaContactLists.ToList()) });
        }

        private bool contactExist(int id)
        {
            return _context.WaContactLists.Any(e => e.ContactID == id);
        }

        //[HttpPost]
        //public IActionResult SendWA([FromBody] string numWA)
        //{
        //    try
        //    {
                

        //        return Json(new { success = true, message = "WhatsApp message sent successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions and errors here
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}
    }
}
