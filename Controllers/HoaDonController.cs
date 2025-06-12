using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using NailMVC.ViewModels;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace NailMVC.Controllers
{
    public class HoaDonController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public HoaDonController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "HoaDon";
        }

        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, int? nhanVienId)
        {
            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            using var client = new HttpClient();
            // 👇 Thêm Accept: application/json vào Header
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            // Gọi API lấy tất cả hóa đơn
            var res = await client.GetAsync(_apiUrl);
            if (!res.IsSuccessStatusCode)
                return View(new List<HoaDonViewModel>());

            var content = await res.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<HoaDon>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Ánh xạ về ViewModel
            var result = list.Select(h => new HoaDonViewModel
            {
                Id = h.Id,
                NgayLap = h.NgayLap,
                KhachHangId = h.KhachHangId,
                NhanVienId = h.NhanVienId,
                TongTien = h.TongTien,
                GiamGia = h.GiamGia,
                ThanhTien = h.ThanhTien,
                HinhThucThanhToan = h.HinhThucThanhToan,
                TenKhachHang = h.KhachHang?.HoTen ?? $"#{h.KhachHangId}",
                TenNhanVien = h.NhanVien?.HoTen ?? $"#{h.NhanVienId}",
                DichVus = h.HoaDonDichVus?.Select(d => new DichVuItem
                {
                    DichVuId = d.DichVuId,
                    TenDichVu = d.DichVu?.TenDichVu ?? $"#{d.DichVuId}",
                    Gia = d.DichVu?.Gia ?? 0
                }).ToList() ?? new()
            }).ToList();

            // ✅ Lọc theo ngày
            if (fromDate.HasValue)
                result = result.Where(h => h.NgayLap >= fromDate.Value).ToList();
            if (toDate.HasValue)
                result = result.Where(h => h.NgayLap <= toDate.Value).ToList();

            // ✅ Lọc theo nhân viên
            if (nhanVienId.HasValue)
                result = result.Where(h => h.NhanVienId == nhanVienId.Value).ToList();

            // Để giữ lại giá trị lọc khi hiển thị lại view
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.SelectedNhanVienId = nhanVienId;

            await SetViewBags(client);
            return View(result);
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HoaDon model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ!");

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await client.PostAsync(_apiUrl, content);
            if (res.IsSuccessStatusCode)
                return Ok();

            return BadRequest("Tạo hóa đơn thất bại!");
        }

        public async Task<IActionResult> Delete(int id)
        {
            using var client = new HttpClient();
            var res = await client.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction("Index");
        }

        private async Task SetViewBags(HttpClient client)
        {
            var khRes = await client.GetAsync(_config["NailAPI:BaseUrl"] + "KhachHang");
            var nvRes = await client.GetAsync(_config["NailAPI:BaseUrl"] + "NhanVien");
            var dvRes = await client.GetAsync(_config["NailAPI:BaseUrl"] + "DichVu");

            ViewBag.KhachHangs = khRes.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<List<KhachHang>>(await khRes.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : new List<KhachHang>();

            ViewBag.NhanViens = nvRes.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<List<NhanVien>>(await nvRes.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : new List<NhanVien>();

            ViewBag.DichVus = dvRes.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<List<DichVu>>(await dvRes.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : new List<DichVu>();
        }
    }
}
