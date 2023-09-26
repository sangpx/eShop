using eShop.AdminApp.Services.Interface;
using eShop.ViewModels.Systems.Users;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eShop.AdminApp.Services.Implement
{
    public class UserAPIClient : IUserAPIClient
    {
        private readonly IHttpClientFactory _clientFactory;

        public UserAPIClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> Login(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:5001");
            var response = await client.PostAsync("/api/Users/login", httpContent);
            var token = await response.Content.ReadAsStringAsync();
            return token;
        }
    }
}