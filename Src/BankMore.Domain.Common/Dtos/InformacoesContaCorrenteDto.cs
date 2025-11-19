namespace BankMore.Domain.Common
{
    /// <summary>
    /// Modelo de dados para vizualização de informações da conta.
    /// </summary>
    public class InformacoesContaCorrenteDto
    {
        /// <summary>
        /// Id do titular da conta.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Cpf do titular da conta.
        /// </summary>
        public string Cpf { get; set; }

        /// <summary>
        /// Nome completo do titular da conta.
        /// </summary>
        public string Nome { get; set; }
        /// <summary>
        /// NÃºmero da conta corrente.
        /// </summary>
        public int Numero { get; set; }

        /// <summary>
        /// Indica se a conta está ativa.
        /// </summary>
        public bool Ativo { get; set; }
    }
}
