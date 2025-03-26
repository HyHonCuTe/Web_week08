using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using vodaohuyhoang_buoi3.Models;

namespace vodaohuyhoang_buoi3.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CartController : Controller
    {
        
            private const string CartSessionKey = "Cart";

            // Lấy giỏ hàng từ Session
            private List<CartItem> GetCart()
            {
                var cartJson = HttpContext.Session.GetString(CartSessionKey);
                if (string.IsNullOrEmpty(cartJson))
                {
                    return new List<CartItem>();
                }
                return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            }

            // Lưu giỏ hàng vào Session
            private void SaveCart(List<CartItem> cart)
            {
                var cartJson = JsonConvert.SerializeObject(cart);
                HttpContext.Session.SetString(CartSessionKey, cartJson);
            }

            // Hiển thị giỏ hàng
            public IActionResult Index()
            {
                var cart = GetCart();
                return View(cart);
            }

        // Thêm sản phẩm vào giỏ hàng
        [HttpGet]
        public IActionResult AddToCart(int productId, string productName, decimal price, int quantity = 1)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.Id == productId);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    Id = productId,
                    Name = productName,
                    Price = price,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity += quantity;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng
        [HttpPost]
            public IActionResult UpdateQuantity(int productId, int quantity)
            {
                var cart = GetCart();
                var item = cart.FirstOrDefault(p => p.Id == productId);

                if (item != null && quantity > 0)
                {
                    item.Quantity = quantity;
                    SaveCart(cart);
                }

                return RedirectToAction("Index");
            }

            // Xóa một sản phẩm khỏi giỏ hàng
            [HttpPost]
            public IActionResult Remove(int productId)
            {
                var cart = GetCart();
                var item = cart.FirstOrDefault(p => p.Id == productId);

                if (item != null)
                {
                    cart.Remove(item);
                    SaveCart(cart);
                }

                return RedirectToAction("Index");
            }

            // Xóa toàn bộ giỏ hàng
            public IActionResult Clear()
            {
                HttpContext.Session.Remove(CartSessionKey);
                return RedirectToAction("Index");
            }

            // Tóm tắt giỏ hàng (dùng Partial View)
            public IActionResult CartSummary()
            {
                var cart = GetCart();
                return PartialView("_CartSummary", cart);
            }

            // Đặt hàng
            public IActionResult Checkout()
            {
                var cart = GetCart();
                if (cart.Count == 0)
                {
                    return RedirectToAction("Index");
                }
                return View(cart);
            }

            // Thanh toán (hiển thị mã QR)
            [HttpPost]
            public IActionResult ProcessPayment()
            {
                var cart = GetCart();
                if (cart.Count == 0)
                {
                    return RedirectToAction("Index");
                }

                // Giả lập tạo mã QR (trong thực tế bạn sẽ tích hợp với cổng thanh toán)
                ViewBag.QRCodeUrl = "https://example.com/qr-code.png"; // Thay bằng URL thực tế
                return View("Payment", cart);
            }
        }

    }


