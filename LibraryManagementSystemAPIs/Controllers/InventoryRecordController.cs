using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemAPIs.Controllers
{
    public class InventoryRecordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
