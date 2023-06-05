using Microsoft.AspNetCore.Mvc;

namespace TMS.Web.Controllers
{
    public class LiveDataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
