using Microsoft.AspNetCore.Mvc;

namespace Challenge.Application.Website.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
            
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}