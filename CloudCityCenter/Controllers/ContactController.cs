using Microsoft.AspNetCore.Mvc;

namespace CloudCityCenter.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(string Name, string Email, string Message)
        {
            // Обработка сообщения: можно отправить на почту или в базу
            Console.WriteLine($"Received message from {Name} ({Email}): {Message}");

            TempData["Success"] = "Message sent successfully!";
            return RedirectToAction("Index", "Home"); // или нужная страница
        }
    }
}
