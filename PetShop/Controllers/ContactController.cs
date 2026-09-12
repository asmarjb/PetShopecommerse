using Microsoft.AspNetCore.Mvc;
using PetShop.Models;

namespace PetShopEcommerse.Controllers
{
    public class ContactController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // In a full production environment, this could write to a Messages table or send an email.
            TempData["SuccessMessage"] = "Təşəkkür edirik! Mesajınız uğurla qəbul edildi. Tezliklə sizinlə əlaqə saxlayacağıq.";
            return RedirectToAction(nameof(Index));
        }
    }
}
