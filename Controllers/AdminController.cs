using BistroBookMVC.Models;
using BistroBookMVC.Models.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        // Step 1: Select Date
        public IActionResult ReservationSelectDate()
        {
            ViewData["Title"] = "Select Date";

            return View();
        }

        [HttpPost]
        public IActionResult ReservationSelectDate(DateTime date)
        {
            // Store the date in TempData for use in the next step
            TempData["SelectedDate"] = date;

            return RedirectToAction("ReservationSelectGuestCount");
        }

        // Step 2: Select Guest Count
        public IActionResult ReservationSelectGuestCount()
        {
            ViewData["Title"] = "Select Guest Count";

            TempData.Keep("SelectedDate");

            return View();
        }

        [HttpPost]
        public IActionResult ReservationSelectGuestCount(int guestCount)
        {
            // Store the guest count in TempData for use in the next step
            TempData["GuestCount"] = guestCount;

            return RedirectToAction("ReservationSelectTable");
        }

        // Step 3: Select Table
        public async Task<IActionResult> ReservationSelectTable()
        {
            ViewData["Title"] = "Select Table";

            TempData.Keep("SelectedDate");
            TempData.Keep("GuestCount");

            // Retrieve guest count from TempData
            var guestCount = TempData["GuestCount"] != null ? (int)TempData["GuestCount"] : 1;

            // Get available tables based on guest count
            var response = await _client.GetAsync($"{baseUri}api/Tables/GetAvailableTables{guestCount}");

            Console.WriteLine(guestCount);

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var availableTables = JsonConvert.DeserializeObject<List<Table>>(json);

            ViewBag.AvailableTables = availableTables;

            return View();
        }

        [HttpPost]
        public IActionResult ReservationSelectTable(int tableId)
        {
            // Store the selected table ID in TempData
            TempData["TableId"] = tableId;

            return RedirectToAction("ReservationSelectTime");
        }

        // Step 4: Select Time
        public async Task<IActionResult> ReservationSelectTime()
        {
            ViewData["Title"] = "Select Time";

            TempData.Keep("SelectedDate");
            TempData.Keep("GuestCount");
            TempData.Keep("TableId");

            // Get the selected date and table from TempData
            var date = TempData["SelectedDate"] != null ? (DateTime)TempData["SelectedDate"] : DateTime.Now;
            var id = TempData["TableId"] != null ? (int)TempData["TableId"] : 1;

            // Fetch taken time slots for the selected date and table
            var response = await _client.GetAsync($"{baseUri}api/Reservations/GetReservationsByTableIdAndDate/{id}/{date}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var reservations = JsonConvert.DeserializeObject<List<Reservation>>(json);

            // Create a list to hold all taken times (both start and end times)
            var takenTimeSlots = new HashSet<TimeSpan>();

            // Populate the taken time slots based on reservations
            foreach (var reservation in reservations)
            {
                var startTime = reservation.StartTime;
                var endTime = reservation.EndTime;

                // Mark all time slots between startTime and endTime as taken
                for (var time = startTime; time < endTime; time += new TimeSpan(0, 15, 0)) // 15-minute intervals
                {
                    takenTimeSlots.Add(time);
                }
            }

            // Generate 15-minute interval times from 10:00 to 22:00
            var start = new TimeSpan(10, 0, 0); // 10:00 AM
            var end = new TimeSpan(22, 0, 0);   // 10:00 PM
            var interval = new TimeSpan(0, 15, 0); // 15-minute interval

            List<SelectListItem> timeOptions = new List<SelectListItem>();

            // Create time options
            for (var time = start; time <= end; time += interval)
            {
                timeOptions.Add(new SelectListItem
                {
                    Value = time.ToString(),
                    Text = time.ToString(@"hh\:mm"),
                    Disabled = takenTimeSlots.Contains(time) // Disable if the time is taken
                });
            }

            ViewBag.TimeOptions = timeOptions;
            ViewBag.SelectedDate = date;
            ViewBag.TableId = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReservationSelectTime(TimeSpan startTime, TimeSpan endTime)
        {
            // Ensure TempData values are kept for this request
            TempData.Keep("TableId");
            TempData.Keep("GuestCount");
            TempData.Keep("SelectedDate");

            // Create reservation DTO and send it to the API
            var reservationDto = new CreateReservation
            {
                TableId = (int)TempData["TableId"],
                CustomerId = 1, // Assume you have a customer ID, replace with real data
                GuestCount = (int)TempData["GuestCount"],
                Date = (DateTime)TempData["SelectedDate"],
                StartTime = startTime,
                EndTime = endTime
            };

            var json = JsonConvert.SerializeObject(reservationDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{baseUri}api/Reservations/AddReservation", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ReservationAll"); // Redirect after successful creation
            }

            ModelState.AddModelError("", "Error creating reservation.");
            return View();
        }

        public async Task<IActionResult> ReservationCreate(DateTime date, int tableId)
        {
            ViewData["Title"] = "New Reservation";

            // Generate 15-minute interval times from 10:00 to 22:00
            var start = new TimeSpan(10, 0, 0); // 10:00 AM
            var end = new TimeSpan(22, 0, 0);   // 10:00 PM
            var interval = new TimeSpan(0, 15, 0); // 15 minutes interval

            List<SelectListItem> timeOptions = new List<SelectListItem>();

            // Create time options
            for (var time = start; time <= end; time += interval)
            {
                timeOptions.Add(new SelectListItem
                {
                    Value = time.ToString(),
                    Text = time.ToString(@"hh\:mm"),
                    Disabled = false // Initially, no options are disabled
                });
            }

            // Fetch all reservations
            var response = await _client.GetAsync($"{baseUri}api/Reservations/GetAllReservations");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var allReservations = JsonConvert.DeserializeObject<List<Reservation>>(json);

                // Filter reservations for the selected date and table
                var existingReservations = allReservations
                    .Where(r => r.Date.Date == date.Date && r.TableId == tableId)
                    .ToList();

                // Disable taken time slots
                foreach (var reservation in allReservations)
                {
                    var takenStartTime = reservation.StartTime;
                    var takenEndTime = reservation.EndTime;

                    // Disable the time slots based on existing reservations
                    for (var time = takenStartTime; time < takenEndTime; time += interval)
                    {
                        var item = timeOptions.FirstOrDefault(t => t.Value == time.ToString());
                        if (item != null)
                        {
                            item.Disabled = true; // Mark the time slot as disabled
                        }
                    }
                }
            }

            // Pass the time options to the view using ViewBag
            ViewBag.TimeOptions = timeOptions;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResCreate(CreateReservation res)
        {
            if (!ModelState.IsValid)
            {
                return View(res);
            }
            var json = JsonConvert.SerializeObject(res);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{baseUri}api/Reservations/AddReservation", content);

            return RedirectToAction("ReservationAll");
        }

        public async Task<IActionResult> ReservationEdit(int Id)
        {
            var response = await _client.GetAsync($"{baseUri}api/Reservations/GetReservationById/{Id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var reservation = JsonConvert.DeserializeObject<CreateReservation>(json);

            // Generate 15-minute interval times from 10:00 to 22:00
            var start = new TimeSpan(10, 0, 0); // 10:00 AM
            var end = new TimeSpan(22, 0, 0);   // 10:00 PM
            var interval = new TimeSpan(0, 15, 0); // 15 minutes interval

            List<SelectListItem> timeOptions = new List<SelectListItem>();

            for (var time = start; time <= end; time += interval)
            {
                timeOptions.Add(new SelectListItem
                {
                    Value = time.ToString(),
                    Text = time.ToString(@"hh\:mm")
                });
            }

            // Pass the time options to the view using ViewBag
            ViewBag.TimeOptions = timeOptions;

            return View(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> ResEdit(CreateReservation res)
        {
            if (!ModelState.IsValid)
            {
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
        public async Task<IActionResult> Create(Menu menu)
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
        public async Task<IActionResult> Edit(Menu menu)
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
    }
}
