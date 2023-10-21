using eShop.AdminApp.Services.Interface;
using eShop.ViewModels.Common;
using eShop.ViewModels.Systems.Users;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UserAPIClient(IHttpClientFactory clientFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        //CreateCallAsync
        public async Task<ApiResult<bool>> CreateCallAsync(RegisterRequest request)
        {
            var client = _clientFactory.CreateClient();
            //Port goc cua web
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var json = new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
            var jsonString = JsonConvert.SerializeObject(request, json);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            /*
             *  Mỗi lần CreateClient được gọi:
                Một phiên bản mới của HttpClientđược tạo ra.
                Hành động cấu hình được gọi.
             * */
            //Call API
            var response = await client.PostAsync($"/api/users", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
            }
            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        //GetByIdCallAsync
        public async Task<ApiResult<UserViewModel>> GetByIdCallAsync(Guid id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _clientFactory.CreateClient();
            //Port goc cua web
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            //Gan Header vao moi request de Authorization
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", sessions);
            //Call API
            var response = await client.GetAsync($"/api/users/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<UserViewModel>>(body); //chuyen doi kieu
            }
            return JsonConvert.DeserializeObject<ApiErrorResult<UserViewModel>>(body); //chuyen doi kieu
        }

        //GetUsersPagingsCallAsync
        public async Task<ApiResult<PagedResult<UserViewModel>>> GetUsersPagingsCallAsync(GetUserPagingRequest request)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _clientFactory.CreateClient();
            //Port goc cua web
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            //Gan Header vao moi request de Authorization
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", sessions);
            //Call API
            var response = await client.GetAsync($"/api/users/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyWord={request.KeyWord}");
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<UserViewModel>>>(body); //chuyen doi kieu
            return users;
        }

        //LoginCallAsync
        public async Task<ApiResult<string>> LoginCallAsync(LoginRequest request)
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
            //Call API
            var response = await client.PostAsync("/api/users/login", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(await response.Content.ReadAsStringAsync());
            }
            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await response.Content.ReadAsStringAsync());
        }

        //UpdateCallAsync
        public async Task<ApiResult<bool>> UpdateCallAsync(Guid id, UserUpdateRequest request)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _clientFactory.CreateClient();
            //Port goc cua web
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            /*
             *  Mỗi lần CreateClient được gọi:
                Một phiên bản mới của HttpClientđược tạo ra.
                Hành động cấu hình được gọi.
             * */
            //Gan Header vao moi request de Authorization
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", sessions);
            //Call API
            var response = await client.PutAsync($"/api/users/{id}", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result); //chuyen doi kieu
            }
            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result); //chuyen doi kieu
        }
    }
}