using BistroBookMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BistroBookMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        private string baseUri = "https://localhost:7042/";

        public HomeController(ILogger<HomeController> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            List<Menu> menu = new List<Menu>();
            try
            {
                var response = await _client.GetAsync($"{baseUri}api/Menus/GetAllFavoriteMenuDishes");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = $"Failed to load menu item. Status code: {response.StatusCode}";
                    return View();
                }

                var json = await response.Content.ReadAsStringAsync();

                menu = JsonConvert.DeserializeObject<List<Menu>>(json);
            }
            catch
            {
                ViewBag.ErrorMessage = $"Failed to load data from api";
            }


            return View(menu);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
