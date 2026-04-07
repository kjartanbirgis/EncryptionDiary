using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(string serverUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(serverUrl);
        }

        public async Task<bool> Login(string username, byte[] clientHash)
        {
            var request = new { Username = username, ClientHash = Convert.ToBase64String(clientHash) };
            var response = await _httpClient.PostAsJsonAsync("/api/Users/login", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Register(string username, byte[] clientHash)
        {
            var request = new { Username = username, ClientHash = Convert.ToBase64String(clientHash) };
            var response = await _httpClient.PostAsJsonAsync("/api/Users/register", request);
            return response.IsSuccessStatusCode;
        }
    }
}
