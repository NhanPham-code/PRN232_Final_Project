using DTOs.FeedbackDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Service.Interfaces;
using Service.Services;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace UserUI.Controllers
{
    public class ContactController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IUserService _userService;
        public ContactController(IFeedbackService feedbackService, IUserService userService)
        {
            _feedbackService = feedbackService;
            _userService = userService;
        }

        /* ----------------- Trang Contact ------------------------------- */
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("UserToken"); // ✅ Lấy token từ session

            Console.WriteLine("Token: " + token);
            foreach (var key in HttpContext.Session.Keys)
            {
                Console.WriteLine($"Session[{key}] = {HttpContext.Session.GetString(key)}");
            }
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Common");

            var list = await _feedbackService.GetTopFeedbackAsync(5, token);               // ✅ truyền token

            var customerFeedback = await _feedbackService.GetByUserIdAsync(token);
            if (customerFeedback != null)
                ViewBag.CustomerFeedback = customerFeedback;

            Console.WriteLine("Final Token: " + token);

            return View(list);
        }

        /* ----------------- Submit -------------------------------------- */
        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(CreateFeedbackDTO dto)
        {
            var token = HttpContext.Session.GetString("UserToken");
            Console.WriteLine("*******************Token khi SubmitFeedback: " + token);
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            var ok = await _feedbackService.CreateAsync(dto, token); // ✅ truyền token
            if (!ok) ModelState.AddModelError("", "Gửi phản hồi thất bại!");

            return RedirectToAction("Index");
        }

        /* ----------------- Update -------------------------------------- */
        [HttpPost]
        public async Task<IActionResult> Update(UpdateFeedbackDTO dto)
        {
            var token = HttpContext.Session.GetString("UserToken");
            Console.WriteLine("************Token khi UpdateFeedback: " + token); // 👈 check

            if (string.IsNullOrEmpty(token)) return Unauthorized();

            await _feedbackService.UpdateAsync(dto, token);
            return RedirectToAction("Index");
        }

        /* ----------------- Trang List ------------------------------- */
        [HttpGet]
        public async Task<IActionResult> ListFeedback(int page = 1, int pageSize = 5)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Common");

            var feedbacks = await _feedbackService.GetAllAsync(token);

            var json = await _userService.GetUsersAsync(token, "", 1, 1000);
            var jsonDoc = JsonDocument.Parse(json);

            var users = jsonDoc.RootElement.GetProperty("value").EnumerateArray()
                .Select(u => new
                {
                    UserId = u.GetProperty("UserId").GetInt32(),
                    FullName = u.GetProperty("FullName").GetString() ?? "Unknown",
                    Email = u.GetProperty("Email").GetString() ?? "Unknown"
                })
                .ToDictionary(u => u.UserId, u => (u.FullName, u.Email));

            var combined = feedbacks.Select(fb =>
            {
                var userInfo = users.ContainsKey(fb.UserID) ? users[fb.UserID] : ("Unknown", "Unknown");

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

            // Phân trang
            int totalItems = combined.Count;
            var items = combined.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(items);
        }
    }
}
