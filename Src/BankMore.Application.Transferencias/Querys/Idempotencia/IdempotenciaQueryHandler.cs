using AutoMapper;
using BankMore.Application.Transferencias.ViewModels;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Transferencias.Interfaces;
using MediatR;

namespace BankMore.Application.Transferencias.Querys
{
    public class IdempotenciaQueryHandler : IRequestHandler<IdempotenciaViewQuery, Result<IdempotenciaViewModel>>,
        IRequestHandler<IdempotenciaExisteQuery, bool>
    {
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IMapper _mapper;

        public IdempotenciaQueryHandler(IIdempotenciaRepository idempotenciaRepository,
            IMapper mapper)
        {
            _idempotenciaRepository = idempotenciaRepository;
            _mapper = mapper;
        }

        public async Task<Result<IdempotenciaViewModel>> Handle(IdempotenciaViewQuery message, CancellationToken cancellationToken)
        {
            var chave = _idempotenciaRepository.GetById(message.Id);

            if (chave is null) return Result<IdempotenciaViewModel>.Failure("Chave não encontrada", Erro.INVALID_VALUE);

            return Result<IdempotenciaViewModel>.Success(_mapper.Map<IdempotenciaViewModel>(chave));
        }

        public Task<bool> Handle(IdempotenciaExisteQuery message, CancellationToken cancellationToken)
        {
            var chave = _idempotenciaRepository.GetByExpressionAsync(d => d.Id == message.Id);
            return Task.FromResult(chave is null);
        }
    }
}