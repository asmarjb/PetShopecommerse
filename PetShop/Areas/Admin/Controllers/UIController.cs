using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class UIController : Controller
    {
        public IActionResult Forms()
        {
            return View();
        }

        public IActionResult Components()
        {
            return View();
        }

        public IActionResult Alerts()
        {
            return View();
        }

        public IActionResult Modals()
        {
            return View();
        }

        public IActionResult Blank()
        {
            return View();
        }
    }
}

