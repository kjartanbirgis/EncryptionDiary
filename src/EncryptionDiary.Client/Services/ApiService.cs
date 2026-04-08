using EncryptionDiary.Shared.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token;

        public ApiService(string serverUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(serverUrl);
        }
        private void SetToken(string token)
        {

            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization= new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
        }

        public async Task<UserResponse?> Login(string username, byte[] clientHash)
        {
            var request = new { Username = username, ClientHash = Convert.ToBase64String(clientHash) };
            var response = await _httpClient.PostAsJsonAsync("/api/Users/login", request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            SetToken(result.Token);
            return result.User;
        }

        public async Task<UserResponse> Register(string username, byte[] clientHash)
        {
            var request = new { Username = username, ClientHash = Convert.ToBase64String(clientHash) };
            var response = await _httpClient.PostAsJsonAsync("/api/Users/register", request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            SetToken(result.Token);
            return result.User;
        }

        public async Task<Key?> InsertKey(Key key)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Keys/", key);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var result = await response.Content.ReadFromJsonAsync<Key>();
            return result;
        }

        internal async Task<List<Key>> GetAllKeys(Guid userID)
        {
            var response = await _httpClient.GetFromJsonAsync<List<Key>>($"/api/Keys/all?userId={userID}");
            return response ?? new List<Key>();
        }
    }
}
