using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NailMVC.Controllers
{
    public class TaiKhoanController : BaseController
    {
        private readonly string _apiUrl;
        private readonly IConfiguration _config;

        public TaiKhoanController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "TaiKhoan";
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            using var client = new HttpClient();
            var response = await client.GetAsync(_apiUrl);
            if (!response.IsSuccessStatusCode) return View(new List<TaiKhoan>());

            var content = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<TaiKhoan>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(list);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(TaiKhoan model)
        {
            if (!ModelState.IsValid) return View(model);

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Tạo tài khoản thất bại!");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            using var client = new HttpClient();
            // 👇 Thêm Accept: application/json vào Header
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var res = await client.GetAsync($"{_apiUrl}/{id}");
            if (!res.IsSuccessStatusCode) return RedirectToAction("Index");

            var content = await res.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<TaiKhoan>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaiKhoan model)
        {
            if (!ModelState.IsValid) return View(model);

            using var client = new HttpClient();
            // 👇 Thêm Accept: application/json vào Header
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiUrl}/{model.Id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Cập nhật thất bại!");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using var client = new HttpClient();
            await client.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction("Index");
        }
    }
}
