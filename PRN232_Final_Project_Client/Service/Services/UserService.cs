﻿using DTOs.UserDTO;
using Service.BaseService;
using Service.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Service.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(GatewayHttpClient gateway)
        {
            _httpClient = gateway.Client;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result?.Token;
            }
            else
            {
                // Xử lý lỗi đăng nhập
                return null;
            }
        }

        public async Task<bool> CheckUserExists(string email)
        {
            var response = await _httpClient.GetAsync($"/users/check-email-exit?email={Uri.EscapeDataString(email)}");
            if (response.IsSuccessStatusCode)
            {
                var exists = await response.Content.ReadFromJsonAsync<bool>();
                return exists; // Trả về true nếu người dùng đã tồn tại
            }
            else
            {
                // Xử lý lỗi kiểm tra người dùng
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string fullName, string email, string address, string phone, string password)
        {
            var createUserDto = new CreateUserDTO
            {
                FullName = fullName,
                Email = email,
                Address = address,
                PhoneNumber = phone,
                Password = password,
                Role = "Customer" // Mặc định là "Customer"
            };

            var response = await _httpClient.PostAsJsonAsync("/users/register", createUserDto);
            if (response.IsSuccessStatusCode)
            {
                return true; // Trả về true nếu đăng ký thành công
            }
            else
            {
                // Xử lý lỗi đăng ký
                return false;
            }
        }

        public async Task<ReadUserDTO> GetUserInfoAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/users/info");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadFromJsonAsync<ReadUserDTO>();
            return content;
        }
    }
}
