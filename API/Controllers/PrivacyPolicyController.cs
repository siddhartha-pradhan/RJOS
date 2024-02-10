using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers;

public class PrivacyPolicyController : BaseController<PrivacyPolicyController>
{
    public IActionResult Index()
    {
        return View();
    }
}