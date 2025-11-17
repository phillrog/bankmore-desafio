namespace BankMore.Application.ContasCorrentes.ViewModels
{
    /// <summary>
    /// Representa o registro de idempotência utilizado para prevenir o processamento duplicado 
    /// de requisições.
    /// </summary>
    public class IdempotenciaViewModel
    {
        /// <summary>
        /// O ID da Conta Corrente à qual o registro de idempotência pertence.
        /// </summary>
        /// <example>102</example>
        public Guid IdContaCorrente { get; set; }

        /// <summary>
        /// O corpo da requisição original que foi processada e armazenada (geralmente em formato JSON).
        /// </summary>
        /// <example>{"correlationId": "xyz-123", "valor": 500.00}</example>
        public string Requisicao { get; set; }

        /// <summary>
        /// O resultado da execução da requisição original (geralmente o JSON de sucesso ou falha).
        /// </summary>
        /// <example>{"status": "Sucesso", "transacaoId": "987"}</example>
        public string Resultado { get; set; }
    }
}
