using System.Net.Http.Headers;
using System.Text.Json;

namespace PruebaWebApiHierro.Services
{
    public class ReniecService
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;

        public ReniecService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _token = configuration["ReniecApi:Token"];
        }

        public async Task<(string FirstName, string LastNameP, string LastNameM)?> GetUserDataByDni(string dni)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://apiperu.dev/api/dni");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "dni", dni } });

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<JsonElement>(json);
            var data = obj.GetProperty("data");

            return (
                FirstName: data.GetProperty("nombres").GetString(),
                LastNameP: data.GetProperty("apellido_paterno").GetString(),
                LastNameM: data.GetProperty("apellido_materno").GetString()
            );
        }
    }

}
