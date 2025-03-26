using Microsoft.AspNetCore.Mvc;

namespace vodaohuyhoang_buoi3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
