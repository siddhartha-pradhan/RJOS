using System.Diagnostics;
using Application.DTOs.Error;
using Application.DTOs.User;
using Application.Interfaces.Services;
using Common.Utilities;
using Data.Persistence;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserService _userService;

    public HomeController(ILogger<HomeController> logger, IUserService userService)
    {
        _userService = userService; 
        _logger = logger;
    }

    [Authentication]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    //Get Action
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return View();
        }
        else
        {
            return RedirectToAction("Index");
        }
    }

    //Post Action
    [HttpPost]
    public async Task<ActionResult> Login(UserRequestDTO userRequest)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            var isPasswordValidate = await _userService.IsUserAuthenticated(userRequest);

            if (isPasswordValidate == true)
            {
                int userId = await _userService.GetUserId(userRequest);

                HttpContext.Session.SetInt32("UserId", userId);
                return RedirectToAction("Enrollment");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        else
        {
            return RedirectToAction("Login");
        }
    }

    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        HttpContext.Session.Remove("UserId");
        return RedirectToAction("Login");
    }

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