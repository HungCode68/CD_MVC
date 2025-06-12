using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace NailMVC.Controllers
{
    public class LuongNhanVienController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public LuongNhanVienController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "LuongNhanVien";
        }

        public async Task<IActionResult> Index(int? thang, int? nam)
        {
            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            var apiLuong = _config["NailAPI:BaseUrl"] + "LuongNhanVien";
            var apiNhanVien = _config["NailAPI:BaseUrl"] + "NhanVien";

            using var client = new HttpClient();
            var resLuong = await client.GetAsync(apiLuong);
            var resNhanVien = await client.GetAsync(apiNhanVien);

            if (!resLuong.IsSuccessStatusCode || !resNhanVien.IsSuccessStatusCode)
                return View(new List<LuongNhanVienViewModel>());

            var dataLuong = JsonSerializer.Deserialize<List<LuongNhanVien>>(await resLuong.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var dataNhanVien = JsonSerializer.Deserialize<List<NhanVien>>(await resNhanVien.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Gán danh sách nhân viên cho dropdown
            ViewBag.NhanViens = dataNhanVien;

            // Gán lại để giữ giá trị đã chọn
            ViewBag.Thang = thang;
            ViewBag.Nam = nam;

            // Lọc theo tháng và năm nếu có
            if (thang.HasValue)
                dataLuong = dataLuong.Where(l => l.ThangLuong == thang.Value).ToList();
            if (nam.HasValue)
                dataLuong = dataLuong.Where(l => l.NamLuong == nam.Value).ToList();

            var viewModel = dataLuong!.Select(l => new LuongNhanVienViewModel
            {
                Id = l.Id,
                NhanVienId = l.NhanVienId,
                HoTenNhanVien = dataNhanVien?.FirstOrDefault(nv => nv.Id == l.NhanVienId)?.HoTen ?? "Không rõ",
                ThangLuong = l.ThangLuong,
                NamLuong = l.NamLuong,
                PhanTramHoaDon = l.PhanTramHoaDon,
                TienTip = l.TienTip,
                ThuongKhac = l.ThuongKhac,
                GhiChu = l.GhiChu,
                NgayTinhLuong = l.NgayTinhLuong,
                TongHoaDon = l.TongHoaDon,
                TongLuong = l.TongLuong
            }).ToList();

            return View(viewModel);
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
            var model = JsonSerializer.Deserialize<LuongNhanVien>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LuongNhanVien model)
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LuongNhanVien model)
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

        public async Task<IActionResult> Delete(int id)
        {
            using var client = new HttpClient();
            var response = await client.DeleteAsync($"{_apiUrl}/{id}");

            return RedirectToAction("Index");
        }
    }
}
