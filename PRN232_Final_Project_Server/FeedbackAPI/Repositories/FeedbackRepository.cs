using Microsoft.EntityFrameworkCore;
using FeedbackAPI.Data;
using FeedbackAPI.Models;
using FeedbackAPI.DTOs;
using FeedbackAPI.Repositories.Interface;

namespace FeedbackAPI.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly DBContext _context;

        public FeedbackRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks.ToListAsync();
        }

        public async Task<Feedback?> GetByIdAsync(int id)
        {
            return await _context.Feedbacks.FindAsync(id);
        }

        public async Task AddAsync(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var feedback = await GetByIdAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Feedbacks.AnyAsync(f => f.FeedbackID == id);
        }

        public IQueryable<Feedback> GetAllFeedbacks()
        {
            return _context.Feedbacks.AsQueryable();
        }
    }
}