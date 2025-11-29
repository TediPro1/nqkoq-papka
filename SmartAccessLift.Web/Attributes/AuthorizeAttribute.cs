using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartAccessLift.Web.Helpers;

namespace SmartAccessLift.Web.Attributes;

public class AuthorizeAttribute : ActionFilterAttribute
{
    public string? Roles { get; set; }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!SessionHelper.IsAuthenticated(context.HttpContext.Session))
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl = context.HttpContext.Request.Path });
            return;
        }

        if (!string.IsNullOrEmpty(Roles))
        {
            var userRole = SessionHelper.GetUserRole(context.HttpContext.Session);
            var allowedRoles = Roles.Split(',').Select(r => r.Trim()).ToList();

            if (userRole == null || !allowedRoles.Contains(userRole))
            {
                context.Result = new RedirectToActionResult("Index", "Dashboard", null);
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}

