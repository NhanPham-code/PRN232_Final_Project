using DTOs.UserDTO;
using Service.BaseService;
using Service.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Service.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(GatewayHttpClient gateway)
        {
            _httpClient = gateway.Client;
        }

        private void AddBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string?> LoginAsync(string email, string password, string role)
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password,
                Role = role
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
            AddBearerToken(token);

            var response = await _httpClient.GetAsync("/users/info");

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadFromJsonAsync<ReadUserDTO>();
            return content;
        }

        public async Task<bool> UpdateUserProfileAsync(string token, UpdateUserProfileDTO updateUserProfileDto)
        {
            AddBearerToken(token);

            var response = await _httpClient.
                PutAsJsonAsync("/users/update-profile", updateUserProfileDto);

            if (!response.IsSuccessStatusCode)
            {
                // Xử lý lỗi cập nhật
                //throw new Exception("Failed to update user profile.");
                return false;
            }
            return true;
        }

        public async Task<string> GetUsersAsync(string token, string keyword, int currentPage, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            AddBearerToken(token);

            var skip = (currentPage - 1) * pageSize;
            var filter = string.IsNullOrWhiteSpace(keyword)
                ? ""
                : $"$filter=contains(FullName,'{keyword}') or contains(Email,'{keyword}')&";

            var url = $"/users/get?{filter}$top={pageSize}&$skip={skip}&$count=true";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + content);

                // Trả về trực tiếp content (JSON string)
                return content;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            // create reset password DTO
            var resetPasswordDto = new ResetPasswordDTO
            {
                Email = email,
                NewPassword = newPassword
            };

            // send request to API
            var response = await _httpClient.PostAsJsonAsync("/users/forgot-password", resetPasswordDto);
            // read response content and print it to console
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            if(!response.IsSuccessStatusCode)
            {
                return false; // return false if failed
            }
            // return true if success
            return true;
        }

        public async Task<string> GetUserNameByIdAsync(int userId, string token)
        {
            AddBearerToken(token);

            var response = await _httpClient.GetAsync($"/users/get?$filter=UserId eq {userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }

            return null;
        }

    }
}
