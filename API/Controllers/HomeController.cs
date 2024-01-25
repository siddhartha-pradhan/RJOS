using Common.Utilities;
using System.Diagnostics;
using Application.DTOs.User;
using Application.DTOs.Error;
using Application.DTOs.Password;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;

namespace RJOS.Controllers;

public class HomeController : Controller
{
    private readonly IUserService _userService;

    public HomeController(IUserService userService)
    {
        _userService = userService; 
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
        if (HttpContext.Session.GetInt32("UserId") != null) return RedirectToAction("Index", "Notification");
        
        var isPasswordValidate = await _userService.IsUserAuthenticated(userRequest);

        if (!isPasswordValidate) return RedirectToAction("Login");
        
        var userId = await _userService.GetUserId(userRequest);

        HttpContext.Session.SetInt32("UserId", userId);
                
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
        if (changePassword.NewPassword != changePassword.ConfirmNewPassword)
        {
            return View(changePassword);
        }
        
        var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
        
        var isPasswordChanged = await _userService.ChangePassword(userId, changePassword.CurrentPassword, changePassword.NewPassword);

        if (isPasswordChanged)
        {
            return View();
        }

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