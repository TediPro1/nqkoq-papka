using Microsoft.AspNetCore.Http;
using SmartAccessLift.Web.Models.Entities;

namespace SmartAccessLift.Web.Helpers;

public static class SessionHelper
{
    private const string UserIdKey = "UserId";
    private const string UserEmailKey = "UserEmail";
    private const string UserRoleKey = "UserRole";
    private const string UserNameKey = "UserName";

    public static void SetUser(ISession session, User user)
    {
        session.SetInt32(UserIdKey, user.Id);
        session.SetString(UserEmailKey, user.Email);
        session.SetString(UserRoleKey, user.Role);
        session.SetString(UserNameKey, $"{user.FirstName} {user.LastName}");
    }

    public static int? GetUserId(ISession session)
    {
        return session.GetInt32(UserIdKey);
    }

    public static string? GetUserEmail(ISession session)
    {
        return session.GetString(UserEmailKey);
    }

    public static string? GetUserRole(ISession session)
    {
        return session.GetString(UserRoleKey);
    }

    public static string? GetUserName(ISession session)
    {
        return session.GetString(UserNameKey);
    }

    public static bool IsAuthenticated(ISession session)
    {
        return GetUserId(session).HasValue;
    }

    public static bool IsAdmin(ISession session)
    {
        return GetUserRole(session) == "Admin";
    }

    public static void ClearUser(ISession session)
    {
        session.Clear();
    }
}

