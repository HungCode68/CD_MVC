using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class ScheduledReportService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduledReportService> _logger;
    private readonly string _apiUrl = "https://localhost:7233/api/BaoCao/TaoMoi";

    public ScheduledReportService(IServiceProvider serviceProvider, ILogger<ScheduledReportService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRunTime = DateTime.Today.AddDays(1).AddHours(1); // 01:00 AM mỗi ngày
            var delay = nextRunTime - now;

            if (delay.TotalMilliseconds <= 0)
                delay = TimeSpan.FromHours(24); // nếu trễ thì chạy lại sau 24h

            _logger.LogInformation("Đợi đến: {time}", nextRunTime);

            await Task.Delay(delay, stoppingToken);

            try
            {
                using var client = new HttpClient();
                var response = await client.PostAsync(_apiUrl, null, stoppingToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Tạo báo cáo thành công lúc {time}", DateTime.Now);
                }
                else
                {
                    _logger.LogWarning("Tạo báo cáo thất bại: {code}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo báo cáo");
            }

            // Đợi 24h để chạy lại
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
