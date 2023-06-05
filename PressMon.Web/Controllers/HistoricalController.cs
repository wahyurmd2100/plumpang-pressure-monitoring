using Microsoft.AspNetCore.Mvc;

namespace TMS.Web.Controllers
{
    public class HistoricalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
