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
using Microsoft.AspNetCore.Authorization;

namespace PressMon.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TMSContext _context;

        public ProductsController(TMSContext context)
        {
            _context = context;
        }

        // GET: Products
        [Authorize]
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
                var product = (from p in _context.Master_Products select p);//LINQ
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    product = product.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    product = product.Where(m => m.ProductName.Contains(searchValue));
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
                return View(new Product());
            }
            else
            {
                var product = await _context.Master_Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
        }
       

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("ProductId,ProductCode,ProductName,HexColor,DefaultDensity,DefaultTemp,CreateTime,UpdateTime,CreateBy,UpdateBy")] Product product)
        {

            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    product.CreateBy = User.Identity.Name;
                    product.CreateTime = DateTime.Now;
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        product.UpdateBy = User.Identity.Name;
                        product.UpdateTime = DateTime.Now;
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductExists(product.ProductId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.Master_Products.ToList())});
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", product) });
        }


        // POST: Products/Delete/5

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Master_Products.FindAsync(id);
            _context.Master_Products.Remove(product);
            await _context.SaveChangesAsync();
            return Json(new { html = Helper.RenderRazorViewString(this, "_ViewAll", _context.Master_Products.ToList()) });
        }

        private bool ProductExists(int id)
        {
          return _context.Master_Products.Any(e => e.ProductId == id);
        }
    }
}
