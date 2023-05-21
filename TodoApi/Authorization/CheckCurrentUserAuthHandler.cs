using Microsoft.AspNetCore.Authorization;

namespace TodoApi.Authorization;

public static class AuthorizationHandlerExtensions
{
    // Add AuthHandler to authorization
    public static AuthorizationBuilder AddCurrentUserHandler(this AuthorizationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationHandler, CheckCurrentUserAuthHandler>();
        return builder;
    }

    // Add authorization requirement to policy
    public static AuthorizationPolicyBuilder RequireCurrentUser(this AuthorizationPolicyBuilder builder)
    {
        return builder.RequireAuthenticatedUser()
            .AddRequirements(new CheckCurrentUserRequirement());
    }

    // Authorization requirement
    private class CheckCurrentUserRequirement : IAuthorizationRequirement
    {
    }

    // Authorization requirement handler
    private class CheckCurrentUserAuthHandler : AuthorizationHandler<CheckCurrentUserRequirement>
    {
        private readonly CurrentUser _currentUser;

        public CheckCurrentUserAuthHandler(CurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CheckCurrentUserRequirement requirement)
        {
            if (_currentUser.User is not null)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}