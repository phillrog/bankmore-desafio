namespace BankMore.Application.Transferencias.Interfaces
{
    public interface ISagaTransferenciaProducerDispatcher
    {// O método recebe o evento (message) e o tópico de destino.
     // O INotification é o seu evento de domínio (TransferenciaIniciadaEvent).
        Task PublishAsync(object message, string topic);
    }
}
