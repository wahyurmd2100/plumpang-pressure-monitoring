using Microsoft.AspNetCore.Mvc;
using PressMon.Web.Data;
using System.Linq;
using System;
using System.Linq.Dynamic.Core;

namespace TMS.Web.Controllers
{
    public class AlarmSettingsController : Controller
    {
        // GET: AlarmSettingsController
        public ActionResult Index()
        {
            return View();
        }
    }
}
