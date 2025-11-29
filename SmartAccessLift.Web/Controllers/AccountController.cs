using Microsoft.AspNetCore.Mvc;
using SmartAccessLift.Web.Helpers;
using SmartAccessLift.Web.Models.ViewModels;
using SmartAccessLift.Web.Services;

namespace SmartAccessLift.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthenticationService _authService;

    public AccountController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        // If already logged in, redirect to dashboard
        if (SessionHelper.IsAuthenticated(HttpContext.Session))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _authService.AuthenticateAsync(model.Email, model.Password);
        if (user != null)
        {
            // Set user in session
            SessionHelper.SetUser(HttpContext.Session, user);

            var redirectUrl = returnUrl ?? Url.Action("Index", "Dashboard");
            return Redirect(redirectUrl ?? "/Dashboard");
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    public IActionResult Register()
    {
        // If already logged in, redirect to dashboard
        if (SessionHelper.IsAuthenticated(HttpContext.Session))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _authService.RegisterAsync(model);
        if (user != null)
        {
            // Auto-login after registration
            SessionHelper.SetUser(HttpContext.Session, user);
            return RedirectToAction("Index", "Dashboard");
        }

        ModelState.AddModelError(string.Empty, "Email already registered.");
        return View(model);
    }

    [HttpGet]
    [HttpPost]
    public IActionResult Logout()
    {
        SessionHelper.ClearUser(HttpContext.Session);
        return RedirectToAction("Login", "Account");
    }
}

