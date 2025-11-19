namespace BankMore.Application.ContasCorrentes.ViewModels
{
    /// <summary>
    /// Representa o registro de idempotência utilizado para prevenir o processamento duplicado 
    /// de requisiçÃµes.
    /// </summary>
    public class IdempotenciaViewModel
    {
        /// <summary>
        /// O ID da Idempotência.
        /// </summary>
        /// <example>66185bcf-092e-4019-9c96-a6d264b57fab</example>
        public Guid Id { get; set; }

        /// <summary>
        /// O ID da Conta Corrente Ã  qual o registro de idempotência pertence.
        /// </summary>
        /// <example>66185bcf-092e-4019-9c96-a6d264b57fab</example>
        public Guid IdContaCorrente { get; set; }

        /// <summary>
        /// O corpo da requisição original que foi processada e armazenada (geralmente em formato JSON).
        /// </summary>
        /// <example>{"ABC": "xyz-123", "CDE": 500.00}</example>
        public string Requisicao { get; set; }

        /// <summary>
        /// O resultado da execução da requisição original (geralmente o JSON de sucesso ou falha).
        /// </summary>
        /// <example>{"ABC": "Sucesso", "CDE": "987"}</example>
        public string Resultado { get; set; }

        public IdempotenciaViewModel(Guid id, Guid idContaCorrente, string requisicao, string resultado)
        {
            Id = id;
            IdContaCorrente = idContaCorrente;
            Requisicao = requisicao;
            Resultado = resultado;
        }
    }
}
