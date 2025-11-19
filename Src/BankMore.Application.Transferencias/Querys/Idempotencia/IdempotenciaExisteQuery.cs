using MediatR;

namespace BankMore.Application.Transferencias.Querys
{
    public class IdempotenciaExisteQuery : IRequest<bool>
    {
        public Guid Id { get; private set; }
        public IdempotenciaExisteQuery(Guid id)
        {
            Id = id;
        }
    }
}
