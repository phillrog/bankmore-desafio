using AutoMapper;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class MovimentoQueryHandler : IRequestHandler<MovimentoViewQuery, Result<MovimentoViewModel>>,
        IRequestHandler<MovimentoExisteQuery, bool>
    {
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IMapper _mapper;

        public MovimentoQueryHandler(IMovimentoRepository movimentoRepository,
            IMapper mapper)
        {
            _movimentoRepository = movimentoRepository;
            _mapper = mapper;
        }

        public async Task<Result<MovimentoViewModel>> Handle(MovimentoViewQuery message, CancellationToken cancellationToken)
        {
            var chave = _movimentoRepository.GetById(message.Id);

            if (chave is null) return Result<MovimentoViewModel>.Failure("Movimento não encontrada", Erro.INVALID_VALUE);

            return Result<MovimentoViewModel>.Success(_mapper.Map<MovimentoViewModel>(chave));
        }

        public Task<bool> Handle(MovimentoExisteQuery message, CancellationToken cancellationToken)
        {
            var chave = _movimentoRepository.GetByExpression(d => d.Id == message.Id);
            return Task.FromResult(chave is null);
        }
    }
}