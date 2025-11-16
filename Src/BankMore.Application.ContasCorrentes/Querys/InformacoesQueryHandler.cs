using AutoMapper;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Interfaces;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class InformacoesQueryHandler : IRequestHandler<InformacoesQuery, InformacoesViewModel>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMapper _mapper;

        public InformacoesQueryHandler(IContaCorrenteRepository contaCorrenteRepository,
            IMapper mapper)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _mapper = mapper;
        }

        public async Task<InformacoesViewModel> Handle(InformacoesQuery request, CancellationToken cancellationToken)
        {
            var perfilEntity = _contaCorrenteRepository.GetByCpf(request.Cpf);
            return _mapper.Map<InformacoesViewModel>(perfilEntity);
        }
    }
}