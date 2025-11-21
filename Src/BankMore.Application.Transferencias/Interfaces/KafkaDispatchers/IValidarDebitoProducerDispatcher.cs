using BankMore.Domain.Common.Events;

namespace BankMore.Application.Transferencias.Interfaces
{
    public interface IValidarDebitoProducerDispatcher
    {
        /// <summary>
        /// Publica mensagem para ValidarDebitoConsumer
        /// </summary>
        /// <param name="message">MovimentacaoContaRespostaEvent</param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task PublishAsync(MovimentacaoContaRespostaEvent message, string topic);
    }
}
