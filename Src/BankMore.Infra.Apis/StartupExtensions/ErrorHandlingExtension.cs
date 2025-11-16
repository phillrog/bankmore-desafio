using Microsoft.AspNetCore.Builder;

namespace BankMore.Services.Apis.StartupExtensions;

public static class ErrorHandlingExtension
{
    public static IApplicationBuilder UseCustomizedErrorHandling(this IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();

        return app;
    }
}
