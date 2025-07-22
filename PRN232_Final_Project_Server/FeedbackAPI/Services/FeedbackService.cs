using AutoMapper;
using AutoMapper.QueryableExtensions;
using FeedbackAPI.Data;
using FeedbackAPI.DTOs;
using FeedbackAPI.Models;
using FeedbackAPI.Repositories.Interface;
using FeedbackAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FeedbackAPI.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _repo;
        private readonly IMapper _mapper;

        public FeedbackService(IFeedbackRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public IQueryable<ReadFeedbackDTO> GetAllFeedbacks()
        {
            return _repo.GetAllFeedbacks()
                        .ProjectTo<ReadFeedbackDTO>(_mapper.ConfigurationProvider);
        }
        public IQueryable<Feedback> GetAllFeedbacksForOData()
        {
            // Trả về IQueryable<User> để OData có thể truy vấn
            return _repo.GetAllFeedbacks();

        }
        public async Task<ReadFeedbackDTO> CheckExistFBById(int userId)
        {
            var entity = await _repo.GetAllFeedbacks().FirstOrDefaultAsync(f => f.UserID == userId);

            if (entity == null)
            {
                // Người dùng chưa từng feedback
                return new ReadFeedbackDTO
                {
                    UserID = userId,
                    Description = "",
                    SubmittedDate = DateTime.MinValue
                };
            }

            // Người dùng đã feedback, dùng AutoMapper hoặc manual mapping
            return _mapper.Map<ReadFeedbackDTO>(entity);
        }
        public async Task<ReadFeedbackDTO?> GetByUserIdAsync(int userId)
        {
            var feedback = await _repo.GetFeedbacksByUserId(userId).FirstOrDefaultAsync();
            return feedback == null ? null : _mapper.Map<ReadFeedbackDTO>(feedback);
        }

        public async Task<ReadFeedbackDTO> CreateAsync(CreateFeedbackDTO dto)
        {
            var feedback = _mapper.Map<Feedback>(dto);
            await _repo.AddAsync(feedback);
            return _mapper.Map<ReadFeedbackDTO>(feedback);
        }

        public async Task<bool> UpdateAsync(UpdateFeedbackDTO dto)
        {
            if (!await _repo.ExistsAsync(dto.FeedbackID)) return false;
            var feedback = _mapper.Map<Feedback>(dto);
            await _repo.UpdateAsync(feedback);
            return true;
        }

        public async Task<bool> DeleteByUserIdAsync(int userId)
        {
            var feedback = await _repo.GetFeedbacksByUserId(userId).FirstOrDefaultAsync();
            if (feedback == null) return false;

            await _repo.DeleteAsync(feedback.FeedbackID); // ✅ Truyền đúng ID của Feedback
            Console.WriteLine(" ",feedback.FeedbackID);    
            return true;
        }


    }
}