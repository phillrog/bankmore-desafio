using BankMore.Application.Common.Commands;

namespace BankMore.Application.Transferencias.Interfaces
{
    public interface IDebitarContaProducerDispatcher
    {
        /// <summary>
        /// Publica mensagem para DebitarContaConsumer
        /// </summary>
        /// <param name="message">DebitarCommand</param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task PublishAsync(DebitarContaCommand message, string topic);
    }
}
