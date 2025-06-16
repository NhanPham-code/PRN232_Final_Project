using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeedbackAPI.Data;
using FeedbackAPI.Models;
using FeedbackAPI.DTOs;
using FeedbackAPI.Services.Interface;
using Microsoft.AspNetCore.OData.Query;

namespace FeedbackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _repository;

        public FeedbacksController(IFeedbackService repository)
        {
            _repository = repository;
        }

        // GET: api/Users
        [HttpGet]
        [EnableQuery]
        public IQueryable<ReadFeedbackDTO> GetFeedbacks()
        {
            return _repository.GetAllFeedbacks();
        }

        // GET: api/Feedbacks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadFeedbackDTO>> GetById(int id)
        {
            var feedback = await _repository.GetByIdAsync(id);
            if (feedback == null) return NotFound();

            var dto = new ReadFeedbackDTO
            {
                FeedbackID = feedback.FeedbackID,
                UserID = feedback.UserID,
                Description = feedback.Description,
                SubmittedDate = feedback.SubmittedDate
            };

            return Ok(dto);
        }

        // POST: api/Feedbacks
        [HttpPost]
        public async Task<ActionResult<ReadFeedbackDTO>> Create([FromBody] CreateFeedbackDTO model)
        {
            var newId = await _repository.CreateAsync(model);

            var created = new ReadFeedbackDTO
            {

                UserID = model.UserID,
                Description = model.Description,
                SubmittedDate = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(GetById), new { id = newId }, created);
        }

        // PUT: api/Feedbacks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFeedbackDTO model)
        {
            if (id != model.FeedbackID)
                return BadRequest("Mismatched ID");

            var result = await _repository.UpdateAsync(model);
            if (!result) return NotFound();

            return NoContent();
        }

        // DELETE: api/Feedbacks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repository.DeleteAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
