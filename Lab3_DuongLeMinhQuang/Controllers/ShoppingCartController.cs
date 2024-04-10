using Lab3_DuongLeMinhQuang.Helper;
using Lab3_DuongLeMinhQuang.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
			var cart =
			HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			if (cart == null || !cart.Items.Any())
			{
				// Xử lý giỏ hàng trống...
				return RedirectToAction("Index");
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
			return View("OrderCompleted", order.Id); // Trang xác nhận hoàn	thành đơn hàng
}
		public IActionResult RemoveFromCart(int productId)
        {
            var cart =
            HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart is not null)
            {
                cart.RemoveItem(productId);
                // Lưu lại giỏ hàng vào Session sau khi đã xóa mục
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
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
        public async Task<IActionResult> OrderList()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = _context.Orders
                                 .Where(o => o.UserId == user.Id)
                                 .ToList();

            if (!orders.Any())
            {
                // Xử lý khi không có đơn hàng nào
                ViewBag.Message = "Bạn chưa có đơn hàng nào.";
                return View();
            }

            // Tạo ViewModel nếu muốn truyền thêm dữ liệu hoặc thông tin lý giải cho View
            return View(orders); // Gởi danh sách đơn hàng đến View
        }
          public async Task<IActionResult> OrderDetails(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = _context.Orders
                                .Include(o => o.OrderDetails)
                                .ThenInclude(od => od.Product)
                                .FirstOrDefault(o => o.Id == orderId && o.UserId == user.Id);

            if (order == null)
            {
                // Xử lý trường hợp không tìm thấy đơn hàng
                return RedirectToAction("OrderList");
            }

            // Trả về View với model là order đã tìm được
            return View(order);
        }
    }
}


