using AutoMapper;
using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.Querys;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;

namespace BankMore.Application.ContasCorrentes.Services
{
    public class MovimentoService : IMovimentoService
    {
        #region [ SERVIÃOS ]

        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        #endregion

        #region [ CONSTRUTOR ]

        public MovimentoService(
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

        public async Task<Result<MovimentacaoRelaizadaDto>> Cadastrar(MovimentoViewModel movimento)
        {
            var registerCommand = _mapper.Map<CadastrarNovaMovimentacaoCommand>(movimento);
            return await _bus.SendCommand<CadastrarNovaMovimentacaoCommand, Result<MovimentacaoRelaizadaDto>>(registerCommand);
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