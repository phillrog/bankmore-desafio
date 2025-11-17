using AutoMapper;
using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.Querys;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Core.Bus;

namespace BankMore.Application.Idempotencia.Services
{
    public class IdempotenciaService : IIdempotenciaService
    {
        #region [ SERVIÇOS ]

        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        #endregion

        #region [ CONSTRUTOR ]

        public IdempotenciaService(
            IMapper mapper,
            IMediatorHandler bus)
        {
            _mapper = mapper;
            _bus = bus;
        }
        #endregion

        #region [ PESQUISAR ]
        public async Task<bool> Existe(Guid id)
        {
            var query = new IdempotenciaExisteQuery(id);
            var existe = await _bus.SendCommand<IdempotenciaExisteQuery, bool>(query);
            return existe;
        }
        #endregion

        #region [ CADASTRAR ]

        public void Cadastrar(IdempotenciaViewModel idempotencia)
        {
            var registerCommand = _mapper.Map<CadastrarNovaContaCorrenteCommand>(idempotencia);
            _bus.SendCommand(registerCommand);
        }
        #endregion       

        #region [ DISPOSE ]
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
