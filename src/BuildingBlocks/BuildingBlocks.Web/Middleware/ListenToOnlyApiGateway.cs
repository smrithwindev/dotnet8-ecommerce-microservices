using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var signedHeader = context.Request.Headers["Api-Gateway"];

            //check if request is from Api Gateway - if its from Api gateway it won't be null or empty
            if (signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry the service is unavailabe");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
