using AutoMapper;
using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.Querys.ContaCorrente;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.Core.Models;
using MediatR;
using Models = BankMore.Domain.ContasCorrentes.Models;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class InformacoesQueryHandler : IRequestHandler<InformacoesQuery, Result<InformacoesViewModel>>,
        IRequestHandler<SaldoQuery, Result<SaldoDto>>
    {
        #region [ SERVICES ]

        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMapper _mapper;
        private readonly IContaCorrenteService _contaCorrenteService;
        #endregion

        #region [ CONSTRUTOR ]


        public InformacoesQueryHandler(IContaCorrenteRepository contaCorrenteRepository,
            IMapper mapper,
            IContaCorrenteService contaCorrenteService)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _mapper = mapper;
            _contaCorrenteService = contaCorrenteService;
        }
        #endregion

        #region [ INFORMAÃÃES DA CONTA ]

        public async Task<Result<InformacoesViewModel>> Handle(InformacoesQuery request, CancellationToken cancellationToken)
        {
            Models.ContaCorrente conta;

            if (request.Cpf is not null) conta = _contaCorrenteRepository.GetByCpf(request.Cpf);
            else conta = _contaCorrenteRepository.GetByNumero(request.Numero);

            if (conta is null) return Result<InformacoesViewModel>.Failure("Conta não encontrada", Erro.INVALID_VALUE);

            return Result<InformacoesViewModel>.Success(_mapper.Map<InformacoesViewModel>(conta));
        }
        #endregion

        #region [ SALDO ]

        public async Task<Result<SaldoDto>> Handle(SaldoQuery request, CancellationToken cancellationToken)
        {
            #region [ VALIDAÃÃES ]
            var conta = await _contaCorrenteRepository.GetByExpressionAsync(a => a.Numero == request.NumeroConta);

            if (conta is null) return Result<SaldoDto>.Failure("Apenas contas correntes cadastradas podem consultar o saldo", Erro.INVALID_ACCOUNT);
            if (conta.Inativa()) return Result<SaldoDto>.Failure("Apenas contas correntes ativas podem consultar o saldo", Erro.INACTIVE_ACCOUNT);
            #endregion

            var saldo = await _contaCorrenteService.Saldo(conta.Numero);

            return Result<SaldoDto>.Success(_mapper.Map<SaldoDto>(saldo));
        }
        #endregion
    }
}