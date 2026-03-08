using BuildingBlocks.Web.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;


namespace BuildingBlocks.Web.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {

            //declare variables
            string message = "sorry, internal server error occured. Kindly try again";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "error";

            try
            {
                await next(context);

                //check if exception is Too Many Request // 429 status code
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "warning";
                    message = "Too many request made.";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }

                //check if Response is UnAuthorized // 402 status code
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "alert";
                    message = "you are not authorized to access";
                    statusCode = StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statusCode);
                }

                // if Respnose is Forbidden //403 status code
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    message = "You are not allowed /required to access.";
                    statusCode = StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                //Log Original Exceptions /File,Debugger, Console
                LogException.LogExceptions(ex);

                //check if Exception is Timeout // 408 request timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out Of Time";
                    message = "The request took too long to process. Please try again later.";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }

                // if Exception is caught
                // if None of the above exceptions are caught then do the default
                await ModifyHeader(context, title, message, statusCode);
            }
        }
        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            //display scary-free message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title,

            }), CancellationToken.None);
            return;
        }
    }
}
