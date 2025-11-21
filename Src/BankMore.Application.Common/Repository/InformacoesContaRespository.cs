using BankMore.Domain.Common;
using BankMore.Domain.Common.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BankMore.Infra.Data.Common.Repository;

public class InformacoesContaRespository : IInformacoesContaRespository
{
    private const string ConnectionStringName = "BankMoreContaCorrenteDBConnection";
    private readonly IConfiguration _configuration;

    public InformacoesContaRespository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<InformacoesContaCorrenteDto> GetByCpf(string cpf)
    {
        var connectionString = _configuration.GetConnectionString(ConnectionStringName) ?? _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
        using IDbConnection db = new SqlConnection(connectionString);

        if (connectionString is null) return new InformacoesContaCorrenteDto();

        db.Open();

        var sql = @"SELECT 
                             ID
                           , Cpf
                           , Nome
                           , Numero
                           , Ativo  
                      FROM contacorrente 
                     WHERE Cpf = @Cpf";

        var resultado = await db.QueryFirstOrDefaultAsync<InformacoesContaCorrenteDto>(sql, new { Cpf = cpf });

        return resultado;
    }

    public async Task<InformacoesContaCorrenteDto> GetByNumero(int numero)
    {
        var connectionString = _configuration.GetConnectionString(ConnectionStringName);

        if (connectionString is null) return new InformacoesContaCorrenteDto();

        using IDbConnection db = new SqlConnection(connectionString);

        db.Open();

        var sql = @"SELECT 
                             ID
                           , Cpf
                           , Nome
                           , Numero
                           , Ativo  
                      FROM contacorrente 
                     WHERE Numero = @Numero";

        var resultado = await db.QueryFirstOrDefaultAsync<InformacoesContaCorrenteDto>(sql, new { Numero = numero });

        return resultado;
    }
}
