using AutoMapper;
using BankMore.Domain.Common;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.Common.Querys
{
    public class InformacoesContaCorrenteQueryHandler : IRequestHandler<InformacoesContaCorrenteQuery, Result<InformacoesContaCorrenteDto>>
    {
        #region [ SERVICES ]

        private readonly IInformacoesContaRespository _contaCorrenteRepository;
        private readonly IMapper _mapper;
        #endregion

        #region [ CONSTRUTOR ]


        public InformacoesContaCorrenteQueryHandler(IInformacoesContaRespository contaCorrenteRepository,
            IMapper mapper)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _mapper = mapper;
        }
        #endregion

        #region [ INFORMAÇÕES DA CONTA ]

        public async Task<Result<InformacoesContaCorrenteDto>> Handle(InformacoesContaCorrenteQuery request, CancellationToken cancellationToken)
        {
            InformacoesContaCorrenteDto conta;

            try
            {
                if (request.Cpf is not null) conta = await _contaCorrenteRepository.GetByCpf(request.Cpf);
                else conta = await _contaCorrenteRepository.GetByNumero(request.Numero);

                if (conta is null) return Result<InformacoesContaCorrenteDto>.Failure("Conta não encontrada", Erro.INVALID_VALUE);

                return Result<InformacoesContaCorrenteDto>.Success(_mapper.Map<InformacoesContaCorrenteDto>(conta));
            }
            catch (Exception)
            {

                return Result<InformacoesContaCorrenteDto>.Failure("Ops! Não foi possível efetuar esta ação no momento. Por favor tente mais tarde!", Erro.INTERNAL_ERROR);
            }
        }
        #endregion
    }
}