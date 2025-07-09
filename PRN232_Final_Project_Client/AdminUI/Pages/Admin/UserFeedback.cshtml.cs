﻿using DTOs.FeedbackDTO;
using DTOs.UserDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.BaseService;
using Service.Interfaces;
using System.Text.Json;

namespace AdminUI.Pages.Admin
{
    public class UserFeedbackModel : PageModel
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IUserService _userService;

        public UserFeedbackModel(IFeedbackService feedbackService, IUserService userService)
        {
            _feedbackService = feedbackService;
            _userService = userService;
        }

        public List<FeedbackWithUser> FeedbackList { get; set; } = new();

        public async Task OnGet()
        {
            var token = HttpContext.Session.GetString("AdminToken");
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("❌ Token is missing!");
                return;
            }

           

            // Lấy danh sách feedback
            var feedbacks = await _feedbackService.GetAllAsync(token);

            // Gọi API user
            var json = await _userService.GetUsersAsync(token, "", 1, 1000);
            if (string.IsNullOrEmpty(json)) return;

            var jsonDoc = JsonDocument.Parse(json);

            if (!jsonDoc.RootElement.TryGetProperty("value", out var valueElement))
            {
                Console.WriteLine("❌ Không tìm thấy key 'value' trong JSON.");
                return;
            }

            var users = valueElement.EnumerateArray()
                .Select(u => new
                {
                    UserId = u.GetProperty("UserId").GetInt32(),
                    FullName = u.GetProperty("FullName").GetString() ?? "Unknown",
                    Email = u.GetProperty("Email").GetString() ?? "Unknown"
                })
                .ToDictionary(u => u.UserId, u => (u.FullName, u.Email));

            // Kết hợp feedback + user
            FeedbackList = feedbacks.Select(fb =>
            {
                var userInfo = users.ContainsKey(fb.UserID)
                    ? users[fb.UserID]
                    : ("Unknown", "Unknown");

                return new FeedbackWithUser
                {
                    FeedbackID = fb.FeedbackID,
                    Description = fb.Description,
                    SubmittedDate = fb.SubmittedDate,
                    UserID = fb.UserID,
                    FullName = userInfo.Item1,
                    Email = userInfo.Item2
                };
            }).ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var token = HttpContext.Session.GetString("AdminToken");
            if (string.IsNullOrEmpty(token))
                return new JsonResult(new { success = false, message = "Missing token" });

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("userId", out var userIdElement))
                return new JsonResult(new { success = false, message = "Missing userId" });

            int userId = userIdElement.GetInt32();
            var result = await _feedbackService.DeleteAsync(userId, token);

            return new JsonResult(new { success = result });
        }
    }
}
