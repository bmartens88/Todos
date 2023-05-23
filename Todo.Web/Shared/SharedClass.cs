using System.ComponentModel.DataAnnotations;

namespace Todo.Web.Shared;

public sealed class TodoItem
{
    public int Id { get; set; }

    [Required] public string Title { get; set; } = default!;

    public bool IsComplete { get; set; }
}

public sealed class UserInfo
{
    [Required] public string Username { get; set; } = default!;

    [Required] public string Password { get; set; } = default!;
}

public sealed class ExternalUserInfo
{
    [Required] public string Username { get; set; } = default!;

    [Required] public string ProviderKey { get; set; } = default!;
}

public record AuthToken(string Token);