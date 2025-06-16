using FeedbackAPI.DTOs;
using FeedbackAPI.Models;

namespace FeedbackAPI.Services.Interface
{
    public interface IFeedbackService
    {
        Task<IEnumerable<ReadFeedbackDTO>> GetAllAsync();
        Task<ReadFeedbackDTO?> GetByIdAsync(int id);
        Task<ReadFeedbackDTO> CreateAsync(CreateFeedbackDTO dto);
        Task<bool> UpdateAsync(UpdateFeedbackDTO dto);
        Task<bool> DeleteAsync(int id);
        IQueryable<ReadFeedbackDTO> GetAllFeedbacks();
    }
}