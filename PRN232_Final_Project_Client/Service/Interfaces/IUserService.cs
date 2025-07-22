using DTOs.UserDTO;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<string?> LoginAsync(string email, string password, string role);

        Task<bool> CheckUserExists(string email);

        Task<bool> RegisterAsync(string fullName, string email, string address, string phone, string password);

        Task<bool> ResetPasswordAsync(string email, string newPassword);

        Task<ReadUserDTO> GetUserInfoAsync(string token);

        Task<bool> UpdateUserProfileAsync(string token, UpdateUserProfileDTO updateUserProfileDto);

        // Get users for Admin
        Task<string> GetUsersAsync(string token, string keyword = "", int page = 1, int pageSize = 5);
        Task<string> GetUserNameByIdAsync(int userId, string token);
    }
}
