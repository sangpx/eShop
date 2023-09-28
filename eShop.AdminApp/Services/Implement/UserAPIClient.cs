using eShop.AdminApp.Services.Interface;
using eShop.ViewModels.Common;
using eShop.ViewModels.Systems.Users;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eShop.AdminApp.Services.Implement
{
    public class UserAPIClient : IUserAPIClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public UserAPIClient(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        //Paging User
        public async Task<PagedResult<UserViewModel>> GetUsersPagingsCallAsync(GetUserPagingRequest request)
        {
            var client = _clientFactory.CreateClient();
            //Port goc cua web
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);

            //Gan Header vao moi request de Authorization
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.BearerToken);

            var response = await client.GetAsync($"/api/users/paging?" +
                $"pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyWord={request.KeyWord}");
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<PagedResult<UserViewModel>>(body);
            return users;
        }

        public async Task<string> LoginCallAsync(LoginRequest request)
        {
            //make http requests using ihttpclientfactory in asp.net core
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            /*
             *  Mỗi lần CreateClient được gọi:
                Một phiên bản mới của HttpClientđược tạo ra.
                Hành động cấu hình được gọi.
             * */
            var client = _clientFactory.CreateClient();
            //Port goc cua web
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/users/login", httpContent);
            var token = await response.Content.ReadAsStringAsync();
            return token;
        }
    }
}