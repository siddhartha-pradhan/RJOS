using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers;

public class PrivacyPolicyController : BaseController<NotificationController>
{
    public IActionResult Index()
    {
        return View();
    }
}