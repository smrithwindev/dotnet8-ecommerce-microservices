using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Web.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            // Allow ALL Swagger-related requests
            if (path.StartsWithSegments("/swagger"))
            {
                await next(context);
                return;
            }

            // Allow development (VERY IMPORTANT)
            var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
            if (env.IsDevelopment())
            {
                await next(context);
                return;
            }

            var signedHeader = context.Request.Headers["Api-Gateway"];

            if (string.IsNullOrEmpty(signedHeader))
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry the service is unavailable");
                return;
            }

            await next(context);
        }
    }
}