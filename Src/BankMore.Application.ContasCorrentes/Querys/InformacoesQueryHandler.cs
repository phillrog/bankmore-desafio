using AutoMapper;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class InformacoesQueryHandler : IRequestHandler<InformacoesQuery, Result<InformacoesViewModel>>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMapper _mapper;

        public InformacoesQueryHandler(IContaCorrenteRepository contaCorrenteRepository,
            IMapper mapper)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _mapper = mapper;
        }

        public async Task<Result<InformacoesViewModel>> Handle(InformacoesQuery request, CancellationToken cancellationToken)
        {
            var conta = _contaCorrenteRepository.GetByCpf(request.Cpf);

            if (conta is null) return Result<InformacoesViewModel>.Failure("Conta não encontrada", Erro.INVALID_VALUE);

            return Result<InformacoesViewModel>.Success(_mapper.Map<InformacoesViewModel>(conta));
        }
    }
}