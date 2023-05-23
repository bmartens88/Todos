using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Todo.Web.Server.Authentication;
using Todo.Web.Shared;

namespace Todo.Web.Server;

public static class AuthApi
{
    public static RouteGroupBuilder MapAuth(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth");

        group.MapPost("register", async (UserInfo userInfo, AuthClient client) =>
        {
            var token = await client.GetTokenAsync(userInfo);

            if (token is null)
                return Results.Unauthorized();

            return SignIn(userInfo, token);
        });

        group.MapPost("login", async (UserInfo userInfo, AuthClient client) =>
        {
            var token = await client.GetTokenAsync(userInfo);

            if (token is null)
                return Results.Unauthorized();

            return SignIn(userInfo, token);
        });

        group.MapPost("logout",
                async context => { await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); })
            .RequireAuthorization();

        group.MapGet("login/{provider}", (string provider) =>
        {
            // Trigger external login flow by issuing a challenge with the provider name
            return Results.Challenge(new AuthenticationProperties { RedirectUri = $"/auth/signin/{provider}" },
                new[] { provider });
        });

        group.MapGet("signin/{provider}", async (string provider, AuthClient client, HttpContext context) =>
        {
            var result = await context.AuthenticateAsync(AuthenticationSchemes.ExternalScheme);

            if (result.Succeeded)
            {
                var principal = result.Principal;

                var id = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

                var name = (principal.FindFirstValue(ClaimTypes.Email) ?? principal.Identity?.Name)!;

                var token = await client.GetOrCreateUserAsync(provider,
                    new ExternalUserInfo { ProviderKey = id, Username = name });

                if (token is not null) await SignIn(id, name, token, provider).ExecuteAsync(context);
            }

            // Delete external cookie
            await context.SignOutAsync(AuthenticationSchemes.ExternalScheme);

            return Results.Redirect("/");
        });

        return group;
    }

    private static IResult SignIn(UserInfo userInfo, string token)
    {
        return SignIn(userInfo.Username, userInfo.Username, token, null);
    }

    private static IResult SignIn(string userId, string userName, string token, string? providerName)
    {
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
        identity.AddClaim(new Claim(ClaimTypes.Name, userName));

        var properties = new AuthenticationProperties();

        // Store external provider name
        if (providerName is not null)
            properties.SetExternalProvider(providerName);

        var tokens = new[]
        {
            new AuthenticationToken { Name = TokenNames.AccessToken, Value = token }
        };

        properties.StoreTokens(tokens);

        return Results.SignIn(new ClaimsPrincipal(identity),
            properties,
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}