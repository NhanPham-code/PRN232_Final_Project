using AutoMapper;
using AutoMapper.QueryableExtensions;
using FeedbackAPI.DTOs;
using FeedbackAPI.Models;
using FeedbackAPI.Repositories.Interface;
using FeedbackAPI.Services.Interface;

namespace FeedbackAPI.Services.Interface
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

        public async Task<IEnumerable<ReadFeedbackDTO>> GetAllAsync()
        {
            var feedbacks = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ReadFeedbackDTO>>(feedbacks);
        }

        public async Task<ReadFeedbackDTO?> GetByIdAsync(int id)
        {
            var feedback = await _repo.GetByIdAsync(id);
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

        public async Task<bool> DeleteAsync(int id)
        {
            if (!await _repo.ExistsAsync(id)) return false;
            await _repo.DeleteAsync(id);
            return true;
        }

        public IQueryable<ReadFeedbackDTO> GetAllFeedbacks()
        {
            return _repo.GetAllFeedbacks().ProjectTo<ReadFeedbackDTO>(_mapper.ConfigurationProvider);
        }
    }
}