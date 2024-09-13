using Microsoft.AspNetCore.Mvc;

namespace BistroBookMVC.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
