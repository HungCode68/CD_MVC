using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Text;
using System.Text.Json;

namespace NailMVC.Controllers
{
    public class BaoCaoController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public BaoCaoController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "BaoCao";
        }

        // Hiển thị danh sách báo cáo
        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {
            using var client = new HttpClient();

            // Gọi API
            var response = await client.GetAsync(_apiUrl);

            if (!response.IsSuccessStatusCode)
                return View(new List<BaoCao>());

            var content = await response.Content.ReadAsStringAsync();
            var allData = JsonSerializer.Deserialize<List<BaoCao>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<BaoCao>();

            // Lọc theo ngày nếu có
            if (from.HasValue)
                allData = allData.Where(x => x.NgayBaoCao >= from.Value).ToList();

            if (to.HasValue)
                allData = allData.Where(x => x.NgayBaoCao <= to.Value).ToList();

            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            return View(allData);
        }

        // Chi tiết báo cáo
        public async Task<IActionResult> Details(int id)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{_apiUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var content = await response.Content.ReadAsStringAsync();
            var report = JsonSerializer.Deserialize<BaoCao>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(report);
        }

        // Gọi API tạo báo cáo từ stored procedure
        public async Task<IActionResult> TaoBaoCao()
        {
            using var client = new HttpClient();
            var response = await client.PostAsync($"{_apiUrl}/TaoMoi", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Đã tạo báo cáo thành công.";
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Tạo báo cáo thất bại: " + content;
            }

            return RedirectToAction("Index");
        }
    }
}
