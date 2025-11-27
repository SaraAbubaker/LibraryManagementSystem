using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemAPIs.Controllers
{
    public class BorrowRecordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
