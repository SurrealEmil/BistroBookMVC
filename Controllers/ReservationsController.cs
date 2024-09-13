using Microsoft.AspNetCore.Mvc;

namespace BistroBookMVC.Controllers
{
    public class ReservationsController : Controller
    {
        //private readonly HttpClient _httpClient;


        //public ReservationsController(HttpClient httpClient)
        //{
        //    _httpClient = httpClient;
        //}

        public IActionResult Index()
        {
            return View();
        }
    }
}
