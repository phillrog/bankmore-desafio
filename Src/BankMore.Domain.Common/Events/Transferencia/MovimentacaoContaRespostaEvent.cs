using BankMore.Domain.Core.Events;

namespace BankMore.Domain.Common.Events
{
    public class MovimentacaoContaRespostaEvent : Event
    {
        public MovimentacaoContaRespostaEvent()
        {

        }
        public MovimentacaoContaRespostaEvent(string tipoMovimentacao, Guid id, Guid correlationId, Guid idContaCorrenteOrigem, Guid idContaCorrenteDestino, decimal valor, int status, DateTime dataMovimento)
        {
            Id = id;
            IdContaCorrenteOrigem = idContaCorrenteOrigem;
            IdContaCorrenteDestino = idContaCorrenteDestino;
            Valor = valor;
            Status = status;
            CorrelationId = correlationId;
            DataMovimento = dataMovimento;
            TipoMovimentacao = tipoMovimentacao;
        }
        public Guid Id { get; set; }
        public Guid IdContaCorrenteOrigem { get; set; }
        public Guid IdContaCorrenteDestino { get; set; }
        public decimal Valor { get; set; }
        public int Status { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime DataMovimento { get; set; }
        public string TipoMovimentacao { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
        public bool IsCompensation { get; set; } = false;
        public string Topico { get; set; }
        public string Descricao { get; set; }
    }
}