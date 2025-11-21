using AutoMapper;
using BankMore.Application.Common.Querys.ContaCorrente;
using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Domain.Common.Dtos;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class InformacoesQueryHandler : IRequestHandler<SaldoQuery, Result<SaldoDto>>
    {
        #region [ SERVICES ]

        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMapper _mapper;
        private readonly IContaCorrenteService _contaCorrenteService;
        #endregion

        #region [ CONSTRUTOR ]


        public InformacoesQueryHandler(IContaCorrenteRepository contaCorrenteRepository,
            IMapper mapper,
            IContaCorrenteService contaCorrenteService,
            IInformacoesContaRespository informacoesContaRespository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _mapper = mapper;
            _contaCorrenteService = contaCorrenteService;
        }
        #endregion       

        #region [ SALDO ]

        public async Task<Result<SaldoDto>> Handle(SaldoQuery request, CancellationToken cancellationToken)
        {
            #region [ VALIDAÇÕES ]
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