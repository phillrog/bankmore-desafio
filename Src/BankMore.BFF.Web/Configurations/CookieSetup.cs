namespace BankMore.BFF.Web.Configurations
{
    public static class CookieSetup
    {
        public static IServiceCollection AddCookieConfig(this IServiceCollection services)
        {
            // --- Configuração de Cookies OIDC/Sessão para ambiente HTTP Local ---
            services.Configure<CookiePolicyOptions>(options =>
            {
                // Essencial para permitir que o navegador envie cookies de SameSite=Lax/Unspecified em HTTP
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                {
                    // Se não for HTTPS, defina Secure=false para evitar que o navegador bloqueie o cookie.
                    if (cookieContext.Context.Request.IsHttps == false)
                    {
                        cookieContext.CookieOptions.Secure = false;
                    }
                };
            });
            return services;
        }

        public static IApplicationBuilder UseCookieConfig(this IApplicationBuilder app)
        {
            // UseCookiePolicy deve vir antes da autenticação/bff
            app.UseCookiePolicy();
            return app;
        }
    }
}
