using Todo.Web.Shared;

namespace Todo.Web.Server;

public sealed class AuthClient
{
    private readonly HttpClient _client;

    public AuthClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<string?> GetTokenAsync(UserInfo userInfo)
    {
        var response = await _client.PostAsJsonAsync("users/token", userInfo);

        if (!response.IsSuccessStatusCode)
            return null;

        var token = await response.Content.ReadFromJsonAsync<AuthToken>();

        return token?.Token;
    }

    public async Task<string?> CreatUserAsync(UserInfo userInfo)
    {
        var response = await _client.PostAsJsonAsync("users", userInfo);

        if (!response.IsSuccessStatusCode)
            return null;

        return await GetTokenAsync(userInfo);
    }

    public async Task<string?> GetOrCreateUserAsync(string provider, ExternalUserInfo userInfo)
    {
        var response = await _client.PostAsJsonAsync($"uses/token/{provider}", userInfo);

        if (!response.IsSuccessStatusCode)
            return null;

        var token = await response.Content.ReadFromJsonAsync<AuthToken>();

        return token?.Token;
    }
}