using System.Security.Claims;

namespace ProEventos.API.Extensions;

public static class ClaimsPrincipalExtensios
{
    public static string GetUserNameExtensios(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }
    
    public static int GetUserIdExtensios(this ClaimsPrincipal user)
    {
        return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }
}
