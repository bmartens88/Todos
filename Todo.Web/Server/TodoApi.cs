using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Todo.Web.Server.Authentication;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Todo.Web.Server;

public static class TodoApi
{
    public static RouteGroupBuilder MapTodos(this IEndpointRouteBuilder routes, string todoUri)
    {
        var group = routes.MapGroup("/todos");

        group.RequireAuthorization();

        var transformBuilder = routes.ServiceProvider.GetRequiredService<ITransformBuilder>();
        var transform = transformBuilder.Create(b =>
        {
            b.AddRequestTransform(async c =>
            {
                var accessToken = await c.HttpContext.GetTokenAsync(TokenNames.AccessToken);

                c.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            });
        });

        group.MapForwarder("{*path}", todoUri, new ForwarderRequestConfig(), transform);

        return group;
    }
}