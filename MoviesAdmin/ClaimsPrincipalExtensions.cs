using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user) => 
    user.FindFirstValue(ClaimTypes.NameIdentifier)
    ?? throw new InvalidOperationException("User is not authenticated");

}