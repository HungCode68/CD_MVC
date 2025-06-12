using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace NailMVC.Controllers
{
    public class TheThanhVienController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public TheThanhVienController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "TheThanhVien";
        }

        public async Task<IActionResult> Index(string? maThe)
        {
            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            ViewBag.MaThe = maThe;

            using var client = new HttpClient();

            var url = string.IsNullOrEmpty(maThe)
                ? _apiUrl
                : $"{_apiUrl}?maThe={maThe}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return View(new List<TheThanhVien>());

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<TheThanhVien>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(data);
        }


        public async Task<IActionResult> Details(int id)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{_apiUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<TheThanhVien>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TheThanhVien model)
        {
            if (!ModelState.IsValid) return View(model);

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiUrl, content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Tạo thẻ thất bại");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{_apiUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<TheThanhVien>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TheThanhVien model)
        {
            if (!ModelState.IsValid) return View(model);

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiUrl}/{model.Id}", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Cập nhật thất bại");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using var client = new HttpClient();
            var response = await client.DeleteAsync($"{_apiUrl}/{id}");

            return RedirectToAction("Index");
        }
    }
}
