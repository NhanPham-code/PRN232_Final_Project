using DTOs.CartDTO;
using DTOs.OrderDTO;
using DTOs.ProductDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Service.Interfaces;
using Service.Services;
using System.Text.Json;
using UserUI.Helpers;

namespace UserUI.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;


        public OrderController(IOrderService orderService, IProductService productService, ICartService cartService, INotificationService notificationService, IConfiguration configuration)
        {
            _orderService = orderService;
            _productService = productService;
            _cartService = cartService;
            _notificationService = notificationService;
            _configuration = configuration;
        }

        // GET: /Order
        public async Task<IActionResult> Index(int page = 1, int pageSize = 8)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Common");

            // Calculate skip value for pagination
            int skip = (page - 1) * pageSize;

            var orders = await _orderService.GetOrdersAsync(token, skip, pageSize);

            // Send data using ViewBag
            ViewBag.Orders = orders;
            ViewBag.Token = token;
            ViewBag.CurrentPage = page;
            // ViewBag.TotalCount = totalCount;

            return View("History");
        }

        public async Task<IActionResult> HistoryDetail(string orderJson)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var order = System.Text.Json.JsonSerializer.Deserialize<ReadOrderDTO>(orderJson, options);

            // Tạo List<ReadProductDTO> để chứa thông tin sản phẩm
            var products = new List<ReadProductDTO>();
            var messages = new List<string>();

            // Lặp qua từng OrderDetail để lấy thông tin sản phẩm
            foreach (var detail in order.OrderDetails)
            {
                try
                {
                    Console.WriteLine($"Fetching product details for ProductID: {detail.ProductID}");
                    var product = await _productService.GetProductByIdAsync(detail.ProductID);

                    if (product != null)
                    {
                        products.Add(product);
                    }
                    else
                    {
                        // Tạo một ReadProductDTO giả để hiển thị lỗi
                        products.Add(new ReadProductDTO
                        {
                            ProductID = detail.ProductID,
                            ProductName = "[Sản phẩm không tồn tại]",
                            ImageURL = "/images/no-image.png",
                            Price = 0,
                            StockQuantity = 0
                        });
                        messages.Add($"❌ Sản phẩm có ID {detail.ProductID} không tồn tại.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"[ERROR] Product API failed for ID {detail.ProductID}: {ex.Message}");

                    // Tạo một ReadProductDTO giả để hiển thị lỗi
                    products.Add(new ReadProductDTO
                    {
                        ProductID = detail.ProductID,
                        ProductName = "[Lỗi khi lấy sản phẩm]",
                        ImageURL = "/images/no-image.png",
                        Price = 0,
                        StockQuantity = 0
                    });
                    messages.Add($"❌ Lỗi khi lấy thông tin sản phẩm ID {detail.ProductID}: {ex.Message}");
                }
            }

            ViewBag.Order = order;
            ViewBag.Products = products;
            ViewBag.ProductMessages = messages;

            return View("HistoryDetail", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(string fullName, string email, string phone, string shipping_address, decimal totalPrice, string cartIds, string paymentMethod, string cartJson)
        {
            if (string.IsNullOrEmpty(shipping_address))
            {
                ModelState.AddModelError("shipping_address", "Shipping address is required.");
                return RedirectToAction("Checkout");
            }

            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Common");

            if (paymentMethod == "VNPAY")
            {
                var txnRef = DateTime.Now.Ticks.ToString();
                TempData["OrderTempData"] = JsonConvert.SerializeObject(new
                {
                    fullName,
                    email,
                    phone,
                    shipping_address,
                    totalPrice,
                    cartIds,
                    cartJson,
                    txnRef
                });

                var vnpay = new VnPayLibrary();
                string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                string returnUrl = $"{Request.Scheme}://{Request.Host}/Order/VnpayReturn";
                string tmnCode = _configuration["VNPAY:TmnCode"];
                string hashSecret = _configuration["VNPAY:HashSecret"];

                vnpay.AddRequestData("vnp_Version", "2.1.0");
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", tmnCode);
                vnpay.AddRequestData("vnp_Amount", ((int)(totalPrice * 100000)).ToString());
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {txnRef}");
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
                vnpay.AddRequestData("vnp_TxnRef", txnRef);

                var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, hashSecret);
                return Redirect(paymentUrl);
            }

            // === COD: Create Order Directly ===
            return await CreateOrderAfterPayment(fullName, email, phone, shipping_address, totalPrice, cartIds, cartJson, paymentMethod, token);
        }
        private async Task<IActionResult> CreateOrderAfterPayment(string fullName, string email, string phone, string shipping_address, decimal totalPrice, string cartIds, string cartJson, string paymentMethod, string token)
        {
            var cartIdList = cartIds.Split(',').Select(int.Parse).ToList();

            
            var newOrder = new CreateOrderDTO
            {
                OrderDate = DateTime.UtcNow,
                ShippingAddress = shipping_address,
                TotalAmount = totalPrice,
                OrderStatus = "Pending",
                PaymentMethod = paymentMethod,
                PaymentStatus = "Unpaid",
            };
            if(String.Equals(paymentMethod.ToLower(), "vnpay"))
            {
                newOrder.PaymentStatus = "Paid";
            }

            var orderId = await _orderService.CreateOrderAsync(newOrder, token);

            if (orderId != null)
            {
                var carts = JsonConvert.DeserializeObject<List<CartDTO>>(cartJson);

                foreach (var cart in carts)
                {
                    var product = await _productService.GetProductByIdAsync(cart.ProductID);
                    var unitPrice = product?.Price ?? 0;
                    var newQuantity = product.StockQuantity - cart.Quantity;

                    var detailDto = new CreateOrderDetailDTO
                    {
                        OrderID = orderId.Value,
                        ProductID = cart.ProductID,
                        Quantity = cart.Quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = unitPrice * cart.Quantity,
                    };

                    await _orderService.CreateOrderDetailAsync(detailDto, orderId.Value, token);

                    var updatedProduct = new UpdateProductDTO
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        Description = product.Description,
                        Price = product.Price,
                        StockQuantity = newQuantity,
                        CategoryID = product.CategoryID,
                        ImageURL = product.ImageURL,
                    };

                    await _productService.UpdateProductAsync(updatedProduct, token);
                    await _cartService.DeleteCartAsync(cart.CartID, token);
                }

                TempData["OrderSuccess"] = "Order created successfully!";
                await _notificationService.NotifyCartChanged();
                return RedirectToAction("Index", "Home");
            }

            TempData["OrderErrors"] = "Failed to create order. Please try again.";
            return RedirectToAction("Checkout");
        }
        public async Task<IActionResult> VnpayReturn()
        {
            var vnpay = new VnPayLibrary();
            foreach (string key in Request.Query.Keys)
            {
                vnpay.AddResponseData(key, Request.Query[key]);
            }

            var hashSecret = _configuration["VNPAY:HashSecret"];
            if (vnpay.ValidateSignature(hashSecret))
            {
                string responseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string transactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");

                if (responseCode == "00" && transactionStatus == "00")
                {
                    var temp = TempData["OrderTempData"]?.ToString();
                    if (string.IsNullOrEmpty(temp)) return RedirectToAction("Fail");

                    var orderInfo = JsonConvert.DeserializeObject<dynamic>(temp);

                    return await CreateOrderAfterPayment(
                        (string)orderInfo.fullName,
                        (string)orderInfo.email,
                        (string)orderInfo.phone,
                        (string)orderInfo.shipping_address,
                        (decimal)orderInfo.totalPrice,
                        (string)orderInfo.cartIds,
                        (string)orderInfo.cartJson,
                        "VNPAY",
                        HttpContext.Session.GetString("UserToken")
                    );
                }
            }

            return RedirectToAction("Fail");
        }
        public IActionResult Fail()
        {
            TempData["PaymentFailed"] = "Payment failed! Please try again!";
            return RedirectToAction("Index", "Home");
        }

    }

}
