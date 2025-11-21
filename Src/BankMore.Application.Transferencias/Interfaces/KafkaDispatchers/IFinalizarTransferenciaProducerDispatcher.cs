using BankMore.Application.Transferencias.Commands;

namespace BankMore.Application.Transferencias.Interfaces
{
    public interface IFinalizarTransferenciaProducerDispatcher
    {
        /// <summary>
        /// Publica mensagem para FinalizarTransferenciaConsumer
        /// </summary>
        /// <param name="message">FinalizarTransferenciaCommand</param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task PublishAsync(FinalizarTransferenciaCommand message, string topic);
    }
}
