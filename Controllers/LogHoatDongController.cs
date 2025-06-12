using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Text.Json;

namespace NailMVC.Controllers
{
    public class LogHoatDongController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public LogHoatDongController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "LogHoatDong";
        }

        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {
            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            using var client = new HttpClient();

            string url = _apiUrl;
            if (from.HasValue || to.HasValue)
            {
                var param = new List<string>();
                if (from.HasValue) param.Add($"from={from.Value:yyyy-MM-dd}");
                if (to.HasValue) param.Add($"to={to.Value:yyyy-MM-dd}");
                url += "?" + string.Join("&", param);
            }

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return View(new List<LogHoatDong>());

            var content = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<LogHoatDong>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(list);
        }

    }
}
