using Lab3_DuongLeMinhQuang.Helper;
using Lab3_DuongLeMinhQuang.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lab3_DuongLeMinhQuang.Controllers
{

    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ShoppingCartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }
        public IActionResult AddToCart(int productId, int quantity)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                // Xử lý khi không tìm thấy sản phẩm
                return RedirectToAction("Index", "Product");
            }
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            };
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("AddToCart", "ShoppingCart");
        }
            public IActionResult Checkout()
            {
                return View(new Order());
            }
            [HttpPost]
            public async Task<IActionResult> Checkout(Order order)
            {
                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
                if (cart == null || !cart.Items.Any())
                {
                    // Xử lý giỏ hàng trống...
                    return RedirectToAction("Index", "Product");
                }
                var user = await _userManager.GetUserAsync(User);
                order.UserId = user.Id;
                order.OrderDate = DateTime.UtcNow;
                order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
                order.OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,

                    Price = i.Price
                }).ToList();
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("Cart");
                return RedirectToAction("OrderCompleted", new { orderId = order.Id });
            }
        public IActionResult OrderCompleted(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if(order == null)
            {
                return RedirectToAction("Index", "Product");
            }
            return View(order);
        }

    }
}


