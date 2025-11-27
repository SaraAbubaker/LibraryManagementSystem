using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemAPIs.Controllers
{
    public class AuthorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
