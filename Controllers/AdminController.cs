using BistroBookMVC.Models;
using BistroBookMVC.Models.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace BistroBookMVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {

        private readonly HttpClient _client;
        private string baseUri = "https://localhost:7042/";

        public AdminController(HttpClient client)
        {
            _client = client;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> ReservationAll()
        {
            var response = await _client.GetAsync($"{baseUri}api/Reservations/GetAllReservations");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var reservations = JsonConvert.DeserializeObject<List<Reservation>>(json);

            return View(reservations);
        }

        public async Task<IActionResult> ReservationEdit(int Id)
        {
            var response = await _client.GetAsync($"{baseUri}api/Reservations/GetReservationById/{Id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var reservation = JsonConvert.DeserializeObject<CreateReservation>(json);

            // Use the service to populate ViewBag.TimeOptions
            ViewBag.TimeOptions = GenerateTimeOptions();

            return View(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> ReservationEdit(CreateReservation res)
        {
            if (!ModelState.IsValid)
            {
                // Use the service to populate ViewBag.TimeOptions
                ViewBag.TimeOptions = GenerateTimeOptions();
                return View(res);
            }
            var json = JsonConvert.SerializeObject(res);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{baseUri}api/Reservations/UpdateReservation/{res.Id}", content);

            return RedirectToAction("ReservationAll");
        }

        [HttpPost]
        public async Task<IActionResult> ResDelete(int id)
        {
            var response = await _client.DeleteAsync($"{baseUri}api/Reservations/DeleteReservation/{id}");

            return RedirectToAction("ReservationAll");
        }

        // End of reservation

        // Start of Menu

        public async Task<IActionResult> Menu()
        {
            ViewData["Title"] = "Menu Dishes";

            var response = await _client.GetAsync($"{baseUri}api/Menus/GetAllMenuDishes");

            var json = await response.Content.ReadAsStringAsync();

            var menu = JsonConvert.DeserializeObject<List<Menu>>(json);

            return View(menu);
        }

        public IActionResult MenuCreate()
        {
            ViewData["Title"] = "New Dishes";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MenuCreate(Menu menu)
        {
            if (!ModelState.IsValid)
            {
                return View(menu);
            }
            var json = JsonConvert.SerializeObject(menu);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{baseUri}api/Menus/AddDish", content);

            return RedirectToAction("Menu");
        }

        public async Task<IActionResult> MenuEdit(int id)
        {
            var resopnse = await _client.GetAsync($"{baseUri}api/Menus/GetDishById/{id}");

            var json = await resopnse.Content.ReadAsStringAsync();

            var menu = JsonConvert.DeserializeObject<Menu>(json);

            return View(menu);
        }

        [HttpPost]
        public async Task<IActionResult> MenuEdit(Menu menu)
        {
            if (!ModelState.IsValid)
            {
                return View(menu);
            }
            var json = JsonConvert.SerializeObject(menu);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{baseUri}api/Menus/UpdateMenu/{menu.Id}", content);

            return RedirectToAction("Menu");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _client.DeleteAsync($"{baseUri}api/Menus/DeleteDish/{id}");

            return RedirectToAction("Menu");
        }

        // Helper method at the bottom of the controller
        private List<SelectListItem> GenerateTimeOptions()
        {
            var start = new TimeSpan(10, 0, 0); // 10:00 AM
            var end = new TimeSpan(22, 0, 0);   // 10:00 PM
            var interval = new TimeSpan(0, 30, 0); // 30 minutes interval

            List<SelectListItem> timeOptions = new List<SelectListItem>();

            for (var time = start; time <= end; time += interval)
            {
                timeOptions.Add(new SelectListItem
                {
                    Value = time.ToString(),
                    Text = time.ToString(@"hh\:mm")
                });
            }

            return timeOptions;
        }
    }
}
