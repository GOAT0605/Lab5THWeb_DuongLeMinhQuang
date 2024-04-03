using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Lab3_DuongLeMinhQuang.Areas.Admin.Controllers
{
    [Area("Admin")]//attribute [ Area("Admin")].

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> CreateAdminAccount()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            var user = new IdentityUser
            {
                UserName = "tgdd06105@gmail.com",
                Email = "tgdd06105@gmail.com",
            };

            var result = await _userManager.CreateAsync(user, "Quang@123");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return Content("Admin Account created successfuly");
            }
            return BadRequest("failed to create");

        }
        public IActionResult Index()
        {
            return View();
        }


    }
}


