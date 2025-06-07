using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;
using UserAPI.Repositories;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackRepository _repository;

        public FeedbacksController(IFeedbackRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Feedbacks
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var feedbacks = await _repository.GetAllAsync();
            return Ok(feedbacks);
        }

        // GET: api/Feedbacks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var feedback = await _repository.GetByIdAsync(id);
            if (feedback == null)
                return NotFound();
            return Ok(feedback);
        }

        // POST: api/Feedbacks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Feedback feedback)
        {
            await _repository.AddAsync(feedback);
            return CreatedAtAction(nameof(GetById), new { id = feedback.FeedbackID }, feedback);
        }

        // PUT: api/Feedbacks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Feedback feedback)
        {
            if (id != feedback.FeedbackID)
                return BadRequest();

            if (!await _repository.ExistsAsync(id))
                return NotFound();

            await _repository.UpdateAsync(feedback);
            return NoContent();
        }

        // DELETE: api/Feedbacks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _repository.ExistsAsync(id))
                return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
