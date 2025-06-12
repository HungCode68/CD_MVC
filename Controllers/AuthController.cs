using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BCrypt.Net;

namespace NailMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(TaiKhoan model)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_config["NailAPI:BaseUrl"]);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Lấy toàn bộ danh sách tài khoản
            var response = await client.GetAsync("TaiKhoan");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không thể kết nối đến API.";
                return View(model);
            }

            var danhSach = JsonSerializer.Deserialize<List<TaiKhoan>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Tìm tài khoản theo email
            var user = danhSach.FirstOrDefault(x => x.Email == model.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhau))
            {
                // Đăng nhập thành công
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.VaiTro);
                HttpContext.Session.SetString("TaiKhoanId", user.Id.ToString());
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng.";
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(TaiKhoan model)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_config["NailAPI:BaseUrl"]);

            model.NgayTao = DateTime.Now;
            model.VaiTro = "NhanVien"; // hoặc "User" tùy logic

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("TaiKhoan", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Đăng ký thất bại. Vui lòng thử lại.";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
