using DTOs.OrderDTO;
using DTOs.ProductDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;
using Service.Services;
using System.Net.Http;
using System.Text.Json;

namespace AdminUI.Pages.Admin
{
    public class OrderManagementModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public OrderManagementModel(IOrderService orderService, IProductService productService, IUserService userService)
        {
            _orderService = orderService;
            _productService = productService;
            _userService = userService;
        }


        // navigation from dashboard to order management
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetOrdersAsync(string keyword, int currentPage, int pageSize)
        {
            var token = HttpContext.Session.GetString("AdminToken");
            var jsonResponse = await _orderService.GetAdminOrdersAsync(token, keyword, currentPage, pageSize);

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                return BadRequest(new { error = "Không thể lấy dữ liệu đơn hàng từ API." });
            }

            return Content(jsonResponse, "application/json");
        }


        public async Task<IActionResult> OnGetOrderUpdateAsync(UpdateOrderDTO orderData)
        {
            var token = HttpContext.Session.GetString("AdminToken");

            var result = await _orderService.UpdateOrderAsync(token, orderData);

            if (result)
                return new JsonResult(new { success = true });
            else
                return new JsonResult(new { success = false, message = "Update failed" }) { StatusCode = 500 };
        }

        public async Task<IActionResult> OnGetOrdersWithUserNamesAsync(int userId)
        {
            var token = HttpContext.Session.GetString("AdminToken");

            var user = await _userService.GetUserNameByIdAsync(userId, token);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy người dùng." });
            }

            return new JsonResult(new { success = true, data = user });

        
        }

    }
}
