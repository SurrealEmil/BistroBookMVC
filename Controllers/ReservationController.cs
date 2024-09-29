using Microsoft.AspNetCore.Mvc;

namespace BistroBookMVC.Controllers
{
    public class ReservationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
