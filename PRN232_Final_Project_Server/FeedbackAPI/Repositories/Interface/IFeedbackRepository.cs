using FeedbackAPI.DTOs;
using FeedbackAPI.Models;

namespace FeedbackAPI.Repositories.Interface
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback?> GetByIdAsync(int id);
        Task AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        IQueryable<Feedback> GetAllFeedbacks();
    }
}