using Lab3_DuongLeMinhQuang.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lab3_DuongLeMinhQuang.Controllers
{
	public class HomeController : Controller
	{
		private readonly IProductRepository _productRepository;
		private readonly ILogger<HomeController> _logger;

		public HomeController(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public async Task<IActionResult> Index()
		{
			var products = await _productRepository.GetAllAsync();
			return View(products);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
