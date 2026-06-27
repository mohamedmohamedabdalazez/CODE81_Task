using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace LibraryMS.API.Middleware;

// Delegates actual authentication to BasicAuthMiddleware which runs first and sets context.User.
// This handler just confirms the user is already authenticated.
public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // BasicAuthMiddleware already set context.User if credentials are valid.
        // If user has claims, report success; otherwise report no result (middleware returns 401).
        if (Context.User.Identity?.IsAuthenticated == true)
            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(Context.User, "Basic")));

        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
