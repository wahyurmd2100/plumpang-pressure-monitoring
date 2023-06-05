using Microsoft.AspNetCore.Mvc;

namespace TMS.Web.Controllers
{
    public class AlarmController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
