﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Web.Server.Authentication;

namespace Todo.Web.Server.Pages;

public sealed class IndexModel : PageModel
{
    private readonly ExternalProviders _socialProviders;

    public IndexModel(ExternalProviders socialProviders)
    {
        _socialProviders = socialProviders;
    }

    public string[] ProviderNames { get; set; } = default!;
    public string? CurrentUserName { get; set; }

    public async Task OnGet()
    {
        ProviderNames = await _socialProviders.GetProviderNames();
        CurrentUserName = User.Identity!.Name;
    }
}