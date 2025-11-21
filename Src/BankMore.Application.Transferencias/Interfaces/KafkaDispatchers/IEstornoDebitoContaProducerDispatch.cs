using BankMore.Application.Common.Commands;

namespace BankMore.Application.Transferencias.Interfaces
{
    public interface IEstornoDebitoContaProducerDispatch
    {
        /// <summary>
        /// Publica mensagem para EstornoDebitoConsumer
        /// </summary>
        /// <param name="message">DebitarCommand</param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task PublishAsync(EstornarDebitoContaCommand message, string topic);
    }
}
