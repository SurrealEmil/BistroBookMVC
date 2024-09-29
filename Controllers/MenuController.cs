using BistroBookMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BistroBookMVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly HttpClient _client;
        private string baseUri = "https://localhost:7042/";

        public MenuController(HttpClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Menu Dishes";

            var response = await _client.GetAsync($"{baseUri}api/Menus/GetAllMenuDishes");

            var json = await response.Content.ReadAsStringAsync();

            var menu = JsonConvert.DeserializeObject<List<Menu>>(json);

            return View(menu);
        }
    }
}
