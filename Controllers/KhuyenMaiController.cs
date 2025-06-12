using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NailMVC.Controllers
{
    public class KhuyenMaiController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public KhuyenMaiController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["NailAPI:BaseUrl"] + "KhuyenMai";
        }

        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(_apiUrl);

            if (!response.IsSuccessStatusCode)
                return View(new List<KhuyenMai>());

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<KhuyenMai>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            ViewBag.ApiBaseUrl = _config["NailAPI:BaseUrl"];
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{_apiUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<KhuyenMai>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KhuyenMai km)
        {
            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(km);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
                return Ok();

            return BadRequest(await response.Content.ReadAsStringAsync());
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] KhuyenMai km)
        {
            using var client = new HttpClient();
            // 👇 Thêm Accept: application/json vào Header
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonSerializer.Serialize(km);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiUrl}/{km.Id}", content);

            if (response.IsSuccessStatusCode)
                return Ok();

            return BadRequest(await response.Content.ReadAsStringAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            using var client = new HttpClient();
            var response = await client.DeleteAsync($"{_apiUrl}/{id}");

            if (response.IsSuccessStatusCode)
                return Ok();

            return BadRequest(await response.Content.ReadAsStringAsync());
        }
    }
}
