using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NailMVC.Controllers
{
    public class NhanVienController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public NhanVienController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "NhanVien";
        }

        public async Task<IActionResult> Index(string? soDienThoai)
        {
            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            ViewBag.SoDienThoai = soDienThoai;

            using var client = new HttpClient();
            var url = string.IsNullOrEmpty(soDienThoai) ? _apiUrl : $"{_apiUrl}?soDienThoai={soDienThoai}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return View(new List<NhanVien>());

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<NhanVien>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
            var model = JsonSerializer.Deserialize<NhanVien>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NhanVien model)
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
        public async Task<IActionResult> Create(NhanVien model)
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

        public async Task<IActionResult> Profile()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Auth");

            using var client = new HttpClient();

            // Lấy danh sách nhân viên
            var resNv = await client.GetAsync(_config["NailAPI:BaseUrl"] + "NhanVien");
            if (!resNv.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var nvContent = await resNv.Content.ReadAsStringAsync();
            var nhanViens = JsonSerializer.Deserialize<List<NhanVien>>(nvContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Lấy thông tin nhân viên theo email đăng nhập
            var nhanVien = nhanViens.FirstOrDefault(nv => nv.Email == email);
            if (nhanVien == null)
                return RedirectToAction("Index");

            // Lấy danh sách lương
            var resLuong = await client.GetAsync(_config["NailAPI:BaseUrl"] + "LuongNhanVien");
            if (!resLuong.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var luongContent = await resLuong.Content.ReadAsStringAsync();
            var luongs = JsonSerializer.Deserialize<List<LuongNhanVien>>(luongContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Lọc lương theo nhân viên
            var luongNhanVien = luongs?.Where(l => l.NhanVienId == nhanVien.Id).ToList() ?? new List<LuongNhanVien>();

            // Gửi dữ liệu lương qua ViewBag
            ViewBag.Luong = luongNhanVien;

            // Trả về View cùng thông tin nhân viên
            return View(nhanVien);
        }



    }
}
