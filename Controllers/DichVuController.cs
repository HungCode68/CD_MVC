using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace NailMVC.Controllers
{
    public class DichVuController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public DichVuController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "DichVu";
        }

        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(_apiUrl);

            if (!response.IsSuccessStatusCode)
                return View(new List<DichVu>());

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<DichVu>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DichVu model)
        {
            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiUrl, content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return BadRequest("Thêm dịch vụ thất bại");
        }

        [HttpGet]
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
            var model = JsonSerializer.Deserialize<DichVu>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DichVu model)
        {
            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiUrl}/{model.Id}", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return BadRequest("Cập nhật dịch vụ thất bại");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            using var client = new HttpClient();
            var response = await client.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction("Index");
        }
    }
}
