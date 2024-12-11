using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using ProjetoLO.Domain.Entities;
using System.Net;

namespace ProjetoLO.Infra.IoC.Extensions;

public static class ApiExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandlers(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Request.ContentType = "application/json";

                var contextFeauture = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeauture != null)
                {
                    await context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeauture.Error.Message,
                        Trace = contextFeauture.Error.StackTrace
                    }.ToString());
                }
            });
        });
    }
}