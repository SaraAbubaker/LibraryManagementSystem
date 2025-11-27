using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemAPIs.Controllers
{
    public class AuditBaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
