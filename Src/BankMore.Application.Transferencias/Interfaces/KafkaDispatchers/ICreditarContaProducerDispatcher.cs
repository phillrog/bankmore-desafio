using BankMore.Application.Common.Commands;

namespace BankMore.Application.Transferencias.Interfaces
{
    public interface ICreditarContaProducerDispatcher
    {
        /// <summary>
        /// Publica mensagem para CreditarContaConsumer
        /// </summary>
        /// <param name="message">TentarCreditoCommand</param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task PublishAsync(TentarCreditoCommand message, string topic);
    }
}
