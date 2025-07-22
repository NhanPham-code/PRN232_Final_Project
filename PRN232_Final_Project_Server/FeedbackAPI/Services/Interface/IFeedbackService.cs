using FeedbackAPI.DTOs;
using FeedbackAPI.Models;

namespace FeedbackAPI.Services.Interface
{
    public interface IFeedbackService
    {   
        IQueryable<ReadFeedbackDTO> GetAllFeedbacks(); // Cho OData
        IQueryable<Feedback> GetAllFeedbacksForOData();
        Task<ReadFeedbackDTO?> GetByUserIdAsync(int userId); // Feedback cá nhân
        Task<ReadFeedbackDTO> CreateAsync(CreateFeedbackDTO dto);
        Task<bool> UpdateAsync(UpdateFeedbackDTO dto);
        Task<bool> DeleteByUserIdAsync(int userId);
        Task<ReadFeedbackDTO> CheckExistFBById(int userId);
    }
}