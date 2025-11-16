using AutoMapper;
using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Domain.Core.Bus;

namespace BankMore.Application.ContasCorrentes.Services;

public class ContaCorrenteService : IContaCorrenteService
{
    private readonly IMapper _mapper;
    private readonly IMediatorHandler _bus;

    public ContaCorrenteService(
        IMapper mapper,
        IMediatorHandler bus)
    {
        _mapper = mapper;
        _bus = bus;
    }

    public void Cadastrar(ContaCorrenteViewModel contaCorrenteViewModel)
    {
        var registerCommand = _mapper.Map<CadastrarNovaContaCorrenteCommand>(contaCorrenteViewModel);
        _bus.SendCommand(registerCommand);
    }

    public void Alterar(ContaCorrenteViewModel contaCorrenteViewModel)
    {
        var updateCommand = _mapper.Map<AlterarContaCorrenteCommand>(contaCorrenteViewModel);
        _bus.SendCommand(updateCommand);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
