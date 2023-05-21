using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TodoApi.Users;

public sealed class TodoUser : IdentityUser
{
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