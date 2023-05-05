using CSL.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Linq.Dynamic.Core;
using CSL.Web.Data;
using TMS.Web.Models;
using System.Collections.Generic;
using System;

namespace CSL.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TMSContext _context;
        public HomeController(ILogger<HomeController> logger, TMSContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            InitialHome();
            return View();
        }
        
        public IActionResult GetData()
        {
            var tanks = (from t in _context.Tank_Live_Data
                         join p in _context.Tank on t.TankId equals p.TankId
                         join x in _context.Master_Products on p.ProductId equals x.ProductId
                         select new
                         {
                             t.TankId,
                             p.Name,
                             t.Level
                         });
            return Json(tanks);

        }
        public IActionResult GetDataByTankId(int Id)
        {
            var tanks = (from t in _context.Tank_Live_Data
                         join p in _context.Tank on t.TankId equals p.TankId
                         join x in _context.Master_Products on p.ProductId equals x.ProductId
                         select new
                         {
                             t.TankId,
                             p.Name,
                             x.ProductName,
                             t.Level,
                             t.Temperature,
                             t.GrossVolume,
                             t.NetVolume,
                             t.Density,
                             t.VCF,
                             t.TimeStamp,
                             t.LiquidWeight,
                             persLevel = p.TankHeight != 0? t.Level/p.TankHeight *100:0,
                             persVolume =p.TankVolume !=0? t.NetVolume / p.TankVolume *100:0,
                         });
            var tank = tanks.FirstOrDefault(t => t.TankId == Id);
            return Json(tank);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public void InitialHome()
        {
            var Tanks = (from t in _context.Tank
                         join p in _context.Master_Products on t.ProductId equals p.ProductId
                         select new
                         {
                             t.TankId,
                             t.Name,
                             p.ProductName,
                             p.HexColor
                         }).ToList();
            ViewBag.Tanks = Tanks;
        }
    }
}
