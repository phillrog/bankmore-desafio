using Polly;
using Polly.Extensions.Http;

namespace BankMore.BFF.Web.Configurations
{
    public static class PolicySetup
    {
        // =================================================================
        // 1. POLÍTICAS DO POLLY
        // =================================================================

        /// <summary>
        /// Política de Retentativa: Tenta até 3 vezes, com espera exponencial.
        /// Lida com falhas de rede e status 5xx.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            // Trata erros transitórios (Timeout, DNS, falhas de rede) e códigos 5xx.
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, // Número de tentativas
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 4), // Espera (0.5s, 1s, 2s)
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"[Polly] Tentativa {retryCount}. Falha ao acessar API. Tempo de espera: {timeSpan.TotalSeconds}s.");
                    }
                );
        }

        /// <summary>
        /// Política de Circuit Breaker: Abre o circuito após 5 falhas consecutivas, por 30 segundos.
        /// Evita sobrecarregar uma API doente.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    5, // Número de falhas consecutivas para abrir o circuito
                    TimeSpan.FromSeconds(30), // Tempo que o circuito permanecerá aberto
                    onBreak: (exception, duration) =>
                    {
                        Console.WriteLine($"[Polly] Circuit Breaker Aberto. Duração: {duration}. Causa: {exception.Exception.Message}");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("[Polly] Circuit Breaker Fechado. API recuperada.");
                    }
                );
        }
    }
}
