using DTOs.FeedbackDTO;

namespace Service.Interfaces
{
    public interface IFeedbackService
    {
        /* OData – Trang contact hiển thị carousel */
        Task<List<ReadFeedbackDTO>> GetAllAsync(string token);
        Task<List<ReadFeedbackDTO>> GetAllOdataAsync(string token);
        Task<List<ReadFeedbackDTO>> GetTopFeedbackAsync(int top, string token);
        /* Feedback 1‑user‑1 */
        Task<ReadFeedbackDTO?> GetByUserIdAsync(string token);

        Task<bool> CreateAsync(CreateFeedbackDTO dto, string token);
        Task<bool> UpdateAsync(UpdateFeedbackDTO dto, string token);
        Task<bool> DeleteAsync(int userId, string token);

    }
}
