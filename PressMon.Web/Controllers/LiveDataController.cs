using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PressMon.Web.Data;
using System.Linq;
using TMS.Web.Hubs;

namespace TMS.Web.Controllers
{
    public class LiveDataController : Controller
    {
        private readonly DataContext _context;
        public LiveDataController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoadData()
        {
            var data = _context.LiveDatas.FirstOrDefault(x => x.LocationName == "M-01");
            return Json(data);
        }
    }
}
