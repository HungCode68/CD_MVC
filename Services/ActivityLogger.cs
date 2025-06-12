using System.Net.Http.Json;
using NailMVC.Models;

namespace NailMVC.Services
{
    public class ActivityLogger
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public ActivityLogger(IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

        public async Task LogAsync(int taiKhoanId, string hoatDong, string? duLieuThem = null)
        {
            try
            {
                var client = new HttpClient();
                var log = new LogHoatDong
                {
                    TaiKhoanId = taiKhoanId,
                    HoatDong = hoatDong,
                    ThoiGian = DateTime.Now,
                    DiaChiIP = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                    TrinhDuyet = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"],
                    DuLieuThem = duLieuThem
                };

                var apiUrl = _config["NailAPI:BaseUrl"] + "LogHoatDong";
                var res = await client.PostAsJsonAsync(apiUrl, log);

                if (!res.IsSuccessStatusCode)
                {
                    var error = await res.Content.ReadAsStringAsync();
                    Console.WriteLine("Gửi log thất bại: " + error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi ghi log: " + ex.Message);
            }
        }

    }
}
