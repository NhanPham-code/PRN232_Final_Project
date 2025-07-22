// In UserUI.Controllers/CartController.cs 
using DTOs.CartDTO;
using DTOs.ProductDTO;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Linq;
using System.Collections.Generic; // Add this using directive for List

namespace UserUI.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;


        public CartController(ICartService cartService, IProductService productService, IUserService userService)
        {
            _cartService = cartService;
            _productService = productService;
            _userService = userService;

        }
        // GET: /Cart

        // GET: /Cart 
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Common");

            var totalCartCount = await _cartService.GetCartCountAsync(token);
            ViewBag.TotalCartCount = totalCartCount;

            var initialCarts = await _cartService.GetCartAsync(token, skip: 0, take: 5);

            var products = new List<ReadProductDTO>();
            var updatedCarts = new List<CartQuantityUpdateDTO>();
            var messages = new List<string>();

            // Create a copy to iterate if you're removing items during iteration
            var cartsToProcess = initialCarts.ToList();
            initialCarts.Clear(); // Clear the original list to rebuild it with valid items

            foreach (var cart in cartsToProcess)
            {
                var product = await _productService.GetProductByIdAsync(cart.ProductID);

                if (product != null)
                {
                    if (product.StockQuantity == 0)
                    {
                        await _cartService.DeleteCartAsync(cart.CartID, token);
                        messages.Add($"❌ Sản phẩm **{product.ProductName}** đã hết hàng và bị xoá khỏi giỏ.");
                    }
                    else if (cart.Quantity > product.StockQuantity)
                    {
                        updatedCarts.Add(new CartQuantityUpdateDTO
                        {
                            CartID = cart.CartID,
                            Quantity = product.StockQuantity
                        });
                        cart.Quantity = product.StockQuantity;
                        initialCarts.Add(cart); // Add the modified cart back to the list
                        products.Add(product);
                        messages.Add($"⚠ Số lượng sản phẩm **{product.ProductName}** đã được điều chỉnh xuống còn {product.StockQuantity} do thay đổi tồn kho.");
                    }
                    else
                    {
                        initialCarts.Add(cart); // Add the valid cart back to the list
                        products.Add(product);
                    }
                }
                else
                {
                    await _cartService.DeleteCartAsync(cart.CartID, token);
                    messages.Add($"❌ Một sản phẩm không tồn tại đã bị xoá khỏi giỏ.");
                }
            }

            if (updatedCarts.Any())
            {
                await _cartService.UpdateQuantitiesAsync(updatedCarts, token);
            }

            ViewBag.CartItems = initialCarts;
            ViewBag.Products = products;
            ViewBag.CartWarnings = messages;
            ViewBag.CartCount = initialCarts.Count;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreCartItems(int skip, int take)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            var carts = await _cartService.GetCartAsync(token, skip: skip, take: take);
            var products = new List<ReadProductDTO>();
            var updatedCarts = new List<CartQuantityUpdateDTO>();
            var messages = new List<string>();

            var totalCartCount = await _cartService.GetCartCountAsync(token);

            // Create a copy to iterate if you're removing items during iteration
            var cartsToProcess = carts.ToList();
            carts.Clear(); // Clear the original list to rebuild it with valid items

            foreach (var cart in cartsToProcess)
            {
                var product = await _productService.GetProductByIdAsync(cart.ProductID);

                if (product != null)
                {
                    if (product.StockQuantity == 0)
                    {
                        await _cartService.DeleteCartAsync(cart.CartID, token);
                        messages.Add($"❌ Sản phẩm **{product.ProductName}** đã hết hàng và bị xoá khỏi giỏ.");
                    }
                    else if (cart.Quantity > product.StockQuantity)
                    {
                        updatedCarts.Add(new CartQuantityUpdateDTO
                        {
                            CartID = cart.CartID,
                            Quantity = product.StockQuantity
                        });
                        cart.Quantity = product.StockQuantity;
                        carts.Add(cart); // Add the modified cart back to the list
                        products.Add(product);
                        messages.Add($"⚠ Số lượng sản phẩm **{product.ProductName}** đã được điều chỉnh xuống còn {product.StockQuantity} do thay đổi tồn kho.");
                    }
                    else
                    {
                        carts.Add(cart); // Add the valid cart back to the list
                        products.Add(product);
                    }
                }
                else
                {
                    await _cartService.DeleteCartAsync(cart.CartID, token);
                    messages.Add($"❌ Một sản phẩm không tồn tại đã bị xoá khỏi giỏ.");
                }
            }

            if (updatedCarts.Any())
            {
                await _cartService.UpdateQuantitiesAsync(updatedCarts, token);
            }

            return Json(new { cartItems = carts, products = products, totalCartCount = totalCartCount, warnings = messages });
        }


        [HttpPost]
        public async Task<IActionResult> Add(CartCreateDTO dto)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Common");

            var success = await _cartService.AddCartAsync(dto, token);
            if (!success)
            {
                TempData["OrderErrors"] = "Add to cart failed!";
            }

            return RedirectToAction("Index"); // hoặc quay lại Detail nếu muốn
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartID)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            await _cartService.DeleteCartAsync(cartID, token);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] List<CartQuantityUpdateDTO> updates)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            // In a real application, you might want to fetch the latest cart items from the service
            // after the update to get their new LastUpdated timestamps from the database.
            // For now, we're assuming the client can update this directly.

            var success = await _cartService.UpdateQuantitiesAsync(updates, token);
            if (!success)
            {
                return Ok(new { success = false, message = "Failed to update quantities." });
            }
            return Ok(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> CheckOut(List<int> cartIds)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Common");

            var user = await _userService.GetUserInfoAsync(token);

            if (cartIds == null || cartIds.Count == 0)
            {
                TempData["OrderErrors"] = "No cart selected!";
                return RedirectToAction("Index");
            }

            var allCarts = await _cartService.GetCartAsync(token);
            var selectedCarts = allCarts.Where(c => cartIds.Contains(c.CartID)).ToList();

            var selectedProducts = new List<ReadProductDTO>();
            foreach (var cart in selectedCarts)
            {
                var product = await _productService.GetProductByIdAsync(cart.ProductID);
                selectedProducts.Add(product);
            }

            ViewBag.CartItems = selectedCarts;
            ViewBag.Products = selectedProducts;
            ViewBag.CartIDs = cartIds;
            ViewBag.User = user;

            return View("Checkout");
        }
    }
}