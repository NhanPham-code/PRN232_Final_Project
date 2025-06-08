using System.ComponentModel.DataAnnotations;

namespace FeedbackAPI.DTOs
{
    public class UpdateFeedbackDTO
    {
        [Required]
        public int FeedbackID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
    }
}
