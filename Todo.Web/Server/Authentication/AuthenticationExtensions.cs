using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Todo.Web.Server.Authentication;

public static class AuthenticationExtensions
{
    private static readonly string ExternalProviderKey = "ExternalProviderName";

    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        // Default scheme is cookie
        var authenticationBuilder =
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

        // Add default auth handler which is used between front end and backend
        authenticationBuilder.AddCookie();

        // This is the cookie that will store the user information from the external login provider
        authenticationBuilder.AddCookie(AuthenticationSchemes.ExternalScheme);

        var externalProviders = new Dictionary<string, ExternalAuthProvider>
        {
            ["GitHub"] = static (builder, configure) => builder.AddGitHub(configure)
        };

        foreach (var (providerName, provider) in externalProviders)
        {
            var section = builder.Configuration.GetSection($"Authentication:Schemes:{providerName}");

            if (section.Exists())
            {
                provider(authenticationBuilder, options =>
                {
                    section.Bind(options);

                    if (options is RemoteAuthenticationOptions remoteAuthenticationOptions)
                        remoteAuthenticationOptions.SignInScheme = AuthenticationSchemes.ExternalScheme;
                });
            }
        }

        builder.Services.AddSingleton<ExternalProviders>();

        return builder;
    }

    private static string? GetExternalProvider(this AuthenticationProperties properties)
    {
        return properties.GetString(ExternalProviderKey);
    }

    public static void SetExternalProvider(this AuthenticationProperties properties, string providerName)
    {
        properties.SetParameter(ExternalProviderKey, providerName);
    }

    private delegate void ExternalAuthProvider(AuthenticationBuilder authenticationBuilder, Action<object> configure);
}