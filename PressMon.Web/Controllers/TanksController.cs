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
using PressMon.Web.Models;


namespace PressMon.Web.Controllers
{
    public class TanksController : Controller
    {
        private readonly TMSContext _context;

        public TanksController(TMSContext context)
        {
            _context = context;
        }

        // GET: Tanks
        public IActionResult Index()
        {
            return View();
        }
        //GET  data to datatable
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
                var tank = (from t in _context.Tank
                            join p in _context.Master_Products on t.ProductId equals p.ProductId
                            select new
                            {
                                t.TankId,
                                t.Name,
                                p.ProductName,
                                t.TankVolume,
                                t.TankHeight,
                                t.IsUsed,
                                t.IsAutoPI,
                                t.TankDiameter,
                                t.TankForm,
                                t.HeightSafeCapacity,
                                t.HeightVolMax,
                                t.HeightPointDesk,
                                t.HeightTankBase,
                                t.HeightDeadStock,
                                t.StretchCoefficient,
                                t.DensityCalibrate,
                                t.RaisePerMM,
                                t.DeadstockVolume,
                            });
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    tank = tank.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tank = tank.Where(m => m.ProductName.Contains(searchValue));
                }

                //total number of rows counts   
                recordsTotal = tank.Count();
                //Paging   
                var data = tank.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: Tanks/AddOrEdit
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                populateProductSection();
                return View(new Tank());
            }

            else
            {
                populateProductSection();
                var tank = await _context.Tank.FindAsync(id);
                if (tank == null)
                {
                    return NotFound();
                }
                return View(tank);
            }
        }

        // POST: Tanks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("TankId,Name,ProductId,TankVolume,TankHeight,IsUsed,IsAutoPI,TankDiameter,TankForm,HeightSafeCapacity,HeightVolMax,HeightPointDesk,HeightTankBase,HeightDeadStock,StretchCoefficient,DensityCalibrate,RaisePerMM,DeadstockVolume,CreateTime,CreateBy,UpdateTime,UpdateBy")] Tank tank)
        {

            if (ModelState.IsValid)
            {
                if(id == 0)
                {
                    tank.tankLiveData = new TankLiveData();
                    tank.CreateBy = User.Identity.Name;
                    tank.CreateTime = DateTime.Now;
                    _context.Add(tank);
                    await _context.SaveChangesAsync();
                    
                }
                else
                {
                    try
                    {
                        TankLiveData tankLiveData = _context.Tank_Live_Data.FirstOrDefault(t=>t.TankId == id);
                        if(tankLiveData == null)
                        {
                            tank.tankLiveData = new TankLiveData();
                        }
                        tank.UpdateBy = User.Identity.Name;
                        tank.UpdateTime = DateTime.Now;
                        _context.Update(tank);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TankExists(tank.TankId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.Tank.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", tank) });

        }


        // POST: Products/Delete/5

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tank = await _context.Tank.FindAsync(id);
            var tankLiveData = await _context.Tank_Live_Data.FindAsync(id);
            if (tank != null)
            {
                _context.Tank.Remove(tank);
                await _context.SaveChangesAsync();
            }
            if (tankLiveData != null)
            {
                _context.Tank_Live_Data.Remove(tankLiveData);
                await _context.SaveChangesAsync();
            }           
            
            
            return Json(new { html = Helper.RenderRazorViewString(this, "_ViewAll", _context.Tank.ToList()) });
        }

        private bool TankExists(int id)
        {
          return _context.Tank.Any(e => e.TankId == id);
        }
        private void populateProductSection(object selectedSection = null)
        {
            List<Product> products = new List<Product>();
            products = (from p in _context.Master_Products select p).ToList();
            var productip = new Product()
            {
                ProductId = 0,
                ProductName = "--- Select Product ---"
            };
            products.Insert(0, productip);
            ViewBag.ProductId = products;
        }
    }
}
