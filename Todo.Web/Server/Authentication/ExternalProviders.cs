using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Todo.Web.Server.Authentication;

public sealed class ExternalProviders
{
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private Task<string[]>? _providerNames;

    public ExternalProviders(IAuthenticationSchemeProvider schemeProvider)
    {
        _schemeProvider = schemeProvider;
    }

    public Task<string[]> GetProviderNames()
    {
        return _providerNames ??= GetProviderNamesAsyncCore();
    }

    private async Task<string[]> GetProviderNamesAsyncCore()
    {
        List<string>? providerNames = null;

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        foreach (var scheme in schemes)
        {
            if (scheme.Name == CookieAuthenticationDefaults.AuthenticationScheme ||
                scheme.Name == AuthenticationSchemes.ExternalScheme)
                continue;

            providerNames ??= new List<string>();
            providerNames.Add(scheme.Name);
        }

        return providerNames?.ToArray() ?? Array.Empty<string>();
    }
}