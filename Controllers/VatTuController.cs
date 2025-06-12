using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NailMVC.Controllers
{
    public class VatTuController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public VatTuController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "VatTu";
        }

        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(_apiUrl);

            if (!response.IsSuccessStatusCode)
                return View(new List<VatTu>());

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<VatTu>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            return View(data);
        }

        public async Task<IActionResult> Edit(int id)
        {
            using var client = new HttpClient();
            // 👇 Thêm Accept: application/json vào Header
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync($"{_apiUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<VatTu>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VatTu model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();
            // 👇 Thêm Accept: application/json vào Header
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VatTu model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Thêm mới thất bại");
            return View(model);
        }
    }
}
