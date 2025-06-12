using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace NailMVC.Controllers
{
    public class HoaDonDichVuController : BaseController
    {
        private readonly string _apiUrl;
        private readonly IConfiguration _config;

        public HoaDonDichVuController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "HoaDonDichVu";
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            using var client = new HttpClient();
            var response = await client.GetAsync(_apiUrl);

            if (!response.IsSuccessStatusCode)
                return View(new List<HoaDon_DichVu>());

            var content = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<HoaDon_DichVu>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(list);
        }

        public async Task<IActionResult> Details(int hoaDonId, int dichVuId)
        {
            using var client = new HttpClient();
            var res = await client.GetAsync($"{_apiUrl}/{hoaDonId}/{dichVuId}");
            if (!res.IsSuccessStatusCode) return RedirectToAction("Index");

            var content = await res.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<HoaDon_DichVu>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HoaDon_DichVu model)
        {
            if (!ModelState.IsValid) return View(model);

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await client.PostAsync(_apiUrl, content);
            if (res.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Tạo thất bại!");
            return View(model);
        }

        public async Task<IActionResult> Edit(int hoaDonId, int dichVuId)
        {
            using var client = new HttpClient();
            // 👇 Thêm Accept: application/json vào Header
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var res = await client.GetAsync($"{_apiUrl}/{hoaDonId}/{dichVuId}");
            if (!res.IsSuccessStatusCode) return RedirectToAction("Index");

            var content = await res.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<HoaDon_DichVu>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(HoaDon_DichVu model)
        {
            if (!ModelState.IsValid) return View(model);

            using var client = new HttpClient();
            // 👇 Thêm Accept: application/json vào Header
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await client.PutAsync($"{_apiUrl}/{model.HoaDonId}/{model.DichVuId}", content);
            if (res.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Cập nhật thất bại!");
            return View(model);
        }

        public async Task<IActionResult> Delete(int hoaDonId, int dichVuId)
        {
            using var client = new HttpClient();
            var res = await client.DeleteAsync($"{_apiUrl}/{hoaDonId}/{dichVuId}");
            return RedirectToAction("Index");
        }
    }
}
