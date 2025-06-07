﻿using System.ComponentModel.DataAnnotations;

namespace UserAPI.DTOs
{
    public class CreateFeedbackDTO
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
    }
}
