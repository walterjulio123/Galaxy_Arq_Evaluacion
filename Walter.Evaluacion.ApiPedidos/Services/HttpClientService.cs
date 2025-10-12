
using System.Text.Json;

namespace Walter.Evaluacion.ApiPedidos.Services
{
    public class HttpClientService : IHttpClientService 
    {
        private readonly HttpClient _httpClient;
        public HttpClientService(HttpClient httpClient/*, ILogger<HttpClientService> logger*/)
        {
            _httpClient = httpClient;
            //_logger = logger;
        }
        public async Task<T?> PostAsync<T>(string url, object data)
        {
            try
            {
                //_logger.LogInformation("Making POST request to: {Url}", url);
                var jsonContent = JsonSerializer.Serialize(data);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //_logger.LogInformation("Successfully posted data to: {Url}", url);
                    return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    //_logger.LogWarning("HTTP POST request failed with status: {StatusCode} for URL: {Url}",
                        //response.StatusCode, url);
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error making HTTP POST request to: {Url}", url);
                return default(T);
            }
        }
    }
}
