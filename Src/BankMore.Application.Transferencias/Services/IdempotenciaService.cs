using AutoMapper;
using BankMore.Application.Transferencias.Querys;
using BankMore.Application.Transferencias.Commands;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Application.Transferencias.ViewModels;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;

namespace BankMore.Application.Transferencias.Services
{
    public class IdempotenciaService : IIdempotenciaService
    {
        #region [ SERVIÃOS ]

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

        public async Task<Result<bool>> Cadastrar(IdempotenciaViewModel idempotencia)
        {
            var registerCommand = _mapper.Map<CadastrarNovaIdempotenciaCommand>(idempotencia);
            return await _bus.SendCommand<CadastrarNovaIdempotenciaCommand, Result<bool>>(registerCommand);
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
