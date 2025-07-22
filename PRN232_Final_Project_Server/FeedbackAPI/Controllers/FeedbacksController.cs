using FeedbackAPI.DTOs; 
using FeedbackAPI.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FeedbackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _service;               

        public FeedbacksController(IFeedbackService service)
        {
            _service = service;
        }

        // GET: api/Feedbacks/user/5
        [HttpGet("{userId}")]
        public async Task<ActionResult<ReadFeedbackDTO>> GetByUserId(int userId)
        {
            var dto = await _service.CheckExistFBById(userId);
            return Ok(dto);
        }

        // POST: api/Feedbacks/user/5
        [HttpPost("{userId}")]
        public async Task<ActionResult<ReadFeedbackDTO>> Create(int userId, [FromBody] CreateFeedbackDTO model)
        {
            model.UserID = userId;
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetByUserId), new { userId = created.UserID }, created);
        }

        // PUT: api/Feedbacks/5
        [HttpPut("{userId}/{feedbackId}")]
        public async Task<IActionResult> Update(int userId, int feedbackId, [FromBody] UpdateFeedbackDTO model)
        {
            if (feedbackId != model.FeedbackID)
                return BadRequest("Mismatched ID");
            Console.WriteLine($"Route ID: {feedbackId}, Model.FeedbackID: {model.FeedbackID}"); 
            model.UserID = userId;
            var updated = await _service.UpdateAsync(model);
            return updated ? NoContent() : NotFound();
        }

        // DELETE: api/Feedbacks/user/5
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteByUserId(int userId)
        {
            var deleted = await _service.DeleteByUserIdAsync(userId);
            Console.WriteLine($"❗ Đang cố xoá feedback của userId = {userId}");
            return deleted ? NoContent() : NotFound();
        }

    }
}
