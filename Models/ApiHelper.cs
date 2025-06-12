namespace NailMVC.Models
{
    public static class ApiHelper
    {
        public static HttpClient GetClient(IConfiguration configuration)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(configuration["NailAPI:BaseUrl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }

}
