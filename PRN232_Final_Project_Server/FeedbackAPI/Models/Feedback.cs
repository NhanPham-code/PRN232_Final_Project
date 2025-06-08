using System.ComponentModel.DataAnnotations;

namespace FeedbackAPI.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateTime SubmittedDate { get; set; } = DateTime.Now;
    }
}
