using FeedbackAPI.DTOs;

namespace FeedbackAPI.Services
{
    public interface IFeedbackService
    {
        Task<IEnumerable<ReadFeedbackDTO>> GetAllAsync();
        Task<ReadFeedbackDTO?> GetByIdAsync(int id);
        Task<ReadFeedbackDTO> CreateAsync(CreateFeedbackDTO dto);
        Task<bool> UpdateAsync(UpdateFeedbackDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}