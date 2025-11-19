using AutoMapper;
using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.ContasCorrentes.Interfaces;

namespace BankMore.Application.Transferencia.Querys
{
    public class InformacoesQueryHandler
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

    }
}