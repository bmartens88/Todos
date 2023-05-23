using System.Security.Claims;
using System.Threading.RateLimiting;

namespace TodoApi.Extensions;

public static class RateLimitExtensions
{
    private const string Policy = "PerUserRateLimit";

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(Policy, context =>
            {
                var username = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                return RateLimitPartition.GetTokenBucketLimiter(username, _ => new TokenBucketRateLimiterOptions
                {
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    AutoReplenishment = true,
                    TokenLimit = 100,
                    TokensPerPeriod = 100,
                    QueueLimit = 100
                });
            });
        });
    }

    public static IEndpointConventionBuilder RequirePerUserRateLimiting(this IEndpointConventionBuilder builder)
    {
        return builder.RequireRateLimiting(Policy);
    }
}