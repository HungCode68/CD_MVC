using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Text.Json;
using System.Text;

namespace NailMVC.Controllers
{
    public class ChamCongController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public ChamCongController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "ChamCong";
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];

            using var client = new HttpClient();

            // 1. Lấy danh sách chấm công
            var chamCongResponse = await client.GetAsync(_apiUrl);
            var chamCongList = new List<ChamCong>();

            if (chamCongResponse.IsSuccessStatusCode)
            {
                var content = await chamCongResponse.Content.ReadAsStringAsync();
                chamCongList = JsonSerializer.Deserialize<List<ChamCong>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            // 2. Lấy danh sách nhân viên để hiển thị dropdown
            var nhanVienApi = _config["NailAPI:BaseUrl"] + "NhanVien";
            var nhanVienResponse = await client.GetAsync(nhanVienApi);
            if (nhanVienResponse.IsSuccessStatusCode)
            {
                var nvContent = await nhanVienResponse.Content.ReadAsStringAsync();
                var nhanVienList = JsonSerializer.Deserialize<List<NhanVien>>(nvContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ViewBag.NhanVienList = nhanVienList;
            }
            else
            {
                ViewBag.NhanVienList = new List<NhanVien>();
            }

            return View(chamCongList);
        }


        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ChamCong model)
        {
            model.Ngay = DateTime.Today;
            model.GioVao = DateTime.Now.TimeOfDay;

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiUrl, content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Chấm công thất bại.");
            return View(model);
        }
    }
}
