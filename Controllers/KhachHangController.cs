using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using NailMVC.Services;

namespace NailMVC.Controllers
{
    public class KhachHangController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;
        private readonly ActivityLogger _logger;


        public KhachHangController(IConfiguration config, ActivityLogger logger)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "KhachHang";
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? soDienThoai)
        {
            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            ViewBag.SoDienThoai = soDienThoai;

            using var client = new HttpClient();
            var url = string.IsNullOrEmpty(soDienThoai) ? _apiUrl : $"{_apiUrl}?soDienThoai={soDienThoai}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return View(new List<KhachHang>());

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<KhachHang>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(KhachHang model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var taiKhoanIdStr = HttpContext.Session.GetString("TaiKhoanId");
                Console.WriteLine($"🔍 Session TaiKhoanId: {taiKhoanIdStr}");
                if (int.TryParse(taiKhoanIdStr, out int taiKhoanId))
                {
                    await _logger.LogAsync(taiKhoanId, "Thêm khách hàng", $"Họ tên: {model.HoTen}, SĐT: {model.SoDienThoai}");
                }

                return RedirectToAction("Index");
            }


            ModelState.AddModelError("", "Thêm mới thất bại");
            return View(model);
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
            var model = JsonSerializer.Deserialize<KhachHang>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(KhachHang model)
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


    }
}
