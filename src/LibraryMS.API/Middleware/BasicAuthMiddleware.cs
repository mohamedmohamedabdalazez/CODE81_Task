using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using LibraryMS.Application.Services.Interfaces;

namespace LibraryMS.API.Middleware;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public BasicAuthMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        // Allow OpenAPI/Swagger endpoints without auth
        if (context.Request.Path.StartsWithSegments("/openapi") ||
            context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader) ||
            !AuthenticationHeaderValue.TryParse(authHeader, out var parsed) ||
            !string.Equals(parsed.Scheme, "Basic", StringComparison.OrdinalIgnoreCase) ||
            parsed.Parameter is null)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.WWWAuthenticate = "Basic realm=\"LibraryMS\"";
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Authentication required. Please provide valid Basic Auth credentials."
            });
            return;
        }

        string credentials;
        try
        {
            credentials = Encoding.UTF8.GetString(Convert.FromBase64String(parsed.Parameter));
        }
        catch
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Invalid username or password. Please check your credentials and try again."
            });
            return;
        }

        var colonIndex = credentials.IndexOf(':');
        if (colonIndex < 0)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Invalid username or password. Please check your credentials and try again."
            });
            return;
        }

        var username = credentials[..colonIndex];
        var password = credentials[(colonIndex + 1)..];

        var user = await userService.AuthenticateAsync(username, password);
        if (user is null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Invalid username or password. Please check your credentials and try again."
            });
            return;
        }

        if (!user.IsActive || user.IsDeleted)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Your account has been deactivated. Please contact an administrator."
            });
            return;
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));

        await _next(context);
    }
}
