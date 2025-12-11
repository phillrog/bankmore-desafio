namespace BankMore.BFF.Web.Configurations
{
    public static class ClientsSetup
    {
        public static IServiceCollection AddClientsConfig(this IServiceCollection services)
        {

            services.AddHttpClient("ContasCorrentesAPI", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5001/");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddPolicyHandler(PolicySetup.GetRetryPolicy())
            .AddPolicyHandler(PolicySetup.GetCircuitBreakerPolicy())
            .AddHttpMessageHandler(sp =>
            {

                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

                return new TokenInjectionHandler(httpContextAccessor);
            });
            return services;
        }

        public static IApplicationBuilder UseClientsConfig(this IApplicationBuilder app)
        {
            
            return app;
        }
    }
}
