using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NailMVC.Controllers
{
    public class LichHenController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public LichHenController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "LichHen";
        }

        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(_apiUrl);

            if (!response.IsSuccessStatusCode)
                return View(new List<LichHen>());

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<LichHen>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            // Lấy danh sách nhân viên, khách hàng, dịch vụ
            var baseUrl = _config["NailAPI:BaseUrl"];
            var nhanVienList = await client.GetFromJsonAsync<List<NhanVien>>($"{baseUrl}NhanVien");
            var khachHangList = await client.GetFromJsonAsync<List<KhachHang>>($"{baseUrl}KhachHang");
            var dichVuList = await client.GetFromJsonAsync<List<DichVu>>($"{baseUrl}DichVu");

            ViewBag.NhanViens = nhanVienList;
            ViewBag.KhachHangs = khachHangList;
            ViewBag.DichVus = dichVuList;
            ViewBag.ApiBaseUrl = baseUrl;

            ViewBag.ApiBaseUrl = _apiUrl.Replace("LichHen", "");
            return View(data ?? new List<LichHen>());
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
            var model = JsonSerializer.Deserialize<LichHen>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LichHen model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();
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
        public async Task<IActionResult> Create(LichHen model)
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(_apiUrl);

            if (!response.IsSuccessStatusCode)
                return Json(new List<LichHen>());

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<LichHen>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var events = data.Select(lh => new
            {
                id = lh.Id,
                title = "Lịch hẹn",
                start = lh.NgayHen,
                ghiChu = lh.GhiChu,
                trangThai = lh.TrangThai,
                khachHangId = lh.KhachHangId,
                nhanVienId = lh.NhanVienId,
                dichVuId = lh.DichVuId
            });

            return Json(events);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] LichHen model)
        {
            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiUrl}/{model.Id}", content);

            if (response.IsSuccessStatusCode)
                return Ok();

            return BadRequest("Cập nhật thất bại");
        }


        [HttpGet]
        public async Task<IActionResult> GetChiTiet(int id)
        {
            using var client = new HttpClient();
            var baseUrl = _config["NailAPI:BaseUrl"];

            var lhRes = await client.GetAsync($"{baseUrl}LichHen/{id}");
            if (!lhRes.IsSuccessStatusCode)
                return NotFound();

            var lhData = JsonSerializer.Deserialize<LichHen>(
                await lhRes.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var khRes = await client.GetAsync($"{baseUrl}KhachHang/{lhData.KhachHangId}");
            var nvRes = await client.GetAsync($"{baseUrl}NhanVien/{lhData.NhanVienId}");
            var dvRes = await client.GetAsync($"{baseUrl}DichVu/{lhData.DichVuId}");

            var kh = JsonSerializer.Deserialize<KhachHang>(await khRes.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var nv = JsonSerializer.Deserialize<NhanVien>(await nvRes.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var dv = JsonSerializer.Deserialize<DichVu>(await dvRes.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var vm = new LichHenViewModel
            {
                Id = lhData.Id,
                NgayHen = lhData.NgayHen,
                GhiChu = lhData.GhiChu,
                TrangThai = lhData.TrangThai,
                KhachHangId = lhData.KhachHangId,
                TenKhachHang = kh?.HoTen,
                NhanVienId = lhData.NhanVienId,
                TenNhanVien = nv?.HoTen,
                DichVuId = lhData.DichVuId,
                TenDichVu = dv?.TenDichVu
            };

            return Json(vm);
        }

        private async Task<List<T>> GetList<T>(string endpoint)
        {
            using var client = new HttpClient();
            var res = await client.GetAsync(_config["NailAPI:BaseUrl"] + endpoint);
            if (!res.IsSuccessStatusCode) return new List<T>();
            var content = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


    }
}
