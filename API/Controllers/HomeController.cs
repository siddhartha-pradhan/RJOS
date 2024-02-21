using Common.Utilities;
using System.Diagnostics;
using Application.DTOs.User;
using Application.DTOs.Error;
using Application.DTOs.Password;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using DNTCaptcha.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace RJOS.Controllers;

public class HomeController : BaseController<HomeController>
{
    private Guid UserSessionId;
    private readonly IUserService _userService;
    private readonly IMemoryCache _memoryCache;

    public HomeController(IUserService userService, IMemoryCache memoryCache)
    {
        _userService = userService;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Notification");
        }
        
        var captcha = GenerateAlphanumericCaseSensitiveCaptcha(6);
            
        ViewData["Captcha"] = captcha;
            
        HttpContext.Session.SetString("Captcha", captcha);
            
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(UserRequestDTO userRequest)
    {
        if (string.IsNullOrEmpty(userRequest.UserName) || string.IsNullOrEmpty(userRequest.Password))
        {
            TempData["Warning"] = "Please insert your username and password before submitting your request.";

            return RedirectToAction("Login");
        }
        
        var captcha = HttpContext.Session.GetString("Captcha") ?? "";
        
        if (userRequest.Captcha != captcha)
        {
            TempData["Warning"] = "Invalid captcha, please try again.";
                    
            return RedirectToAction("Login");
        }
           
        var isPasswordValid = await _userService.IsUserAuthenticated(userRequest);

        if (!isPasswordValid)
        {
            TempData["Warning"] = "Invalid username or password.";

            return RedirectToAction("Login");
        }
        
        var userId = await _userService.GetUserId(userRequest);

        var isExist = _memoryCache.TryGetValue(userId, out UserSessionId);

        if (isExist)
        {
            TempData["Warning"] = "The previous session was not logged out.";

            TempData["LogoutAlert"] = "The previous session was not logged out.";

            return RedirectToAction("Login");
        }

        HttpContext.Session.SetInt32("UserId", userId);
                    
        _memoryCache.Set(userId, UserSessionId);
            
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
    
    [HttpGet]
    public ActionResult Logout()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        
        _memoryCache.Remove(userId ?? 1);

        HttpContext.Session.Clear();
        
        HttpContext.Session.Remove("UserId");

        TempData["Success"] = "Successfully logged out.";

        return RedirectToAction("Login");
    }

    [Authentication]
    public IActionResult Enrollment()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RefreshCaptcha()
    {
        var captcha = GenerateAlphanumericCaseSensitiveCaptcha(6);
            
        ViewData["Captcha"] = captcha;
            
        HttpContext.Session.SetString("Captcha", captcha);

        return Json(new
        {
            text = captcha
        });
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
    
    private string GenerateAlphanumericCaseSensitiveCaptcha(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}