using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers;

public class EnrollmentController : Controller
{
    
    public IActionResult Index()
    {
        return View();
    }
}