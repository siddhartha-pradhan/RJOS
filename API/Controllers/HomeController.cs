using Common.Utilities;
using System.Diagnostics;
using Application.DTOs.User;
using Application.DTOs.Error;
using Application.DTOs.Password;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using DNTCaptcha.Core;
using Microsoft.Extensions.Options;

namespace RJOS.Controllers;

public class HomeController : BaseController<HomeController>
{
    private readonly IUserService _userService;
    private readonly IDNTCaptchaValidatorService _validatorService;
    
    public HomeController(IUserService userService, IDNTCaptchaValidatorService validatorService)
    {
        _userService = userService;
        _validatorService = validatorService;
    }

    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return View();
        }

        return RedirectToAction("Index", "Notification");
    }

    [HttpPost]
    public async Task<ActionResult> Login(UserRequestDTO userRequest)
    {
        if (string.IsNullOrEmpty(userRequest.UserName) || string.IsNullOrEmpty(userRequest.Password))
        {
            TempData["Warning"] = "Please insert your username and password before submitting your request.";

            return RedirectToAction("Login");
        }
        
        if (HttpContext.Session.GetInt32("UserId") != null) return RedirectToAction("Index", "Notification");
        
        if (!_validatorService.HasRequestValidCaptchaEntry())
        {
            TempData["Warning"] = "Invalid captcha, please try again.";
            
            return RedirectToAction("Login");
        }
        
        var isPasswordValidate = await _userService.IsUserAuthenticated(userRequest);

        if (!isPasswordValidate)
        {
            TempData["Warning"] = "Invalid username or password.";

            return RedirectToAction("Login");
        }
        
        var userId = await _userService.GetUserId(userRequest);

        HttpContext.Session.SetInt32("UserId", userId);
                
        TempData["Success"] = "Successfully authenticated.";

        return RedirectToAction("Index", "Notification");
    }

    [Authentication]
    public ActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDTO changePassword)
    {
        var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
        
        var isPasswordChanged = await _userService.ChangePassword(userId, changePassword.HdCurrentPassword, changePassword.HdNewPassword);

        if (isPasswordChanged)
        {
            TempData["Success"] = "Password successfully changed.";
            
            HttpContext.Session.Clear();
        
            HttpContext.Session.Remove("UserId");
        
            return RedirectToAction("Login");
        }

        TempData["Warning"] = "Please insert your correct current state of password before changing it.";

        return View(changePassword);
    }
    
    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        
        HttpContext.Session.Remove("UserId");
        
        return RedirectToAction("Login");
    }

    [Authentication]
    public IActionResult Enrollment()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}