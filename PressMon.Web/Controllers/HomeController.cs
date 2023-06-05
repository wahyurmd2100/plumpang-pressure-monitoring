using PressMon.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Linq.Dynamic.Core;
using PressMon.Web.Data;
using PressMon.Web.Models;
using System.Collections.Generic;
using System;

namespace PressMon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        
    }
}
