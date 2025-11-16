using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.ContasCorrentes.Models;
using BankMore.Infra.Data.Common.Repository;
using BankMore.Infra.Data.ContasCorrentes.Context;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Infra.Data.ContasCorrentes.Repository;

public class ContaCorrenteRepository : Repository<ContaCorrente, ApplicationDbContext>, IContaCorrenteRepository
{
    public ContaCorrenteRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public ContaCorrente GetByCpf(string cpf)
    {
        return _dbSet.AsNoTracking().FirstOrDefault(c => c.Cpf.Equals(cpf));
    }

    public ContaCorrente GetByNumero(int numero)
    {
        return _dbSet.AsNoTracking().FirstOrDefault(c => c.Numero == numero);
    }
}
