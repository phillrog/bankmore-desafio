namespace BankMore.Application.ContasCorrentes.ViewModels
{
    /// <summary>
    /// Modelo de dados para vizualização de informações da conta.
    /// </summary>
    public class InformacoesViewModel
    {
        /// <summary>
        /// Nome completo do titular da conta.
        /// </summary>
        public string Nome { get; set; }
        /// <summary>
        /// Número da conta corrente.
        /// </summary>
        public int Numero { get; set; }

        /// <summary>
        /// Indica se a conta está ativa.
        /// </summary>
        public bool Ativo { get; set; }
    }
}
