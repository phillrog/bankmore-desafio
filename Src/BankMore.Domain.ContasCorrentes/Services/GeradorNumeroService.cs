using BankMore.Domain.ContasCorrentes.Interfaces;
using BankMore.Domain.Common.Providers.Hash;

namespace BankMore.Domain.ContasCorrentes.Services;

public class GeradorNumeroService : IGeradorNumeroService
{
    private IPasswordHasher _passwordHasher;

    public GeradorNumeroService(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }
    public int GerarNumeroConta()
    {
        Random random = new Random();
        int numeroBase = random.Next(100000, 999999);
        string numeroContaSemDV = numeroBase.ToString();

        string digitoVerificador = CalcularDigitoVerificador(numeroContaSemDV);
        return Convert.ToInt32($"{numeroContaSemDV}{digitoVerificador}");
    }

    public string CalcularDigitoVerificador(string numero)
    {
        int soma = 0;
        int peso = 2;

        for (int i = numero.Length - 1; i >= 0; i--)
        {
            soma += int.Parse(numero[i].ToString()) * peso;
            peso++;
            if (peso > 9)
            {
                peso = 2;
            }
        }

        int resto = soma % 11;
        if (resto == 0 || resto == 1)
        {
            return "0";
        }
        else
        {
            return (11 - resto).ToString();
        }
    }

    public string GerarSenha(string senha)
    {
        var (hash, salt) = _passwordHasher.Hash(senha);
        return hash;
    }

    public bool ValidarSenha(string senhaAtual, string senhaAnterior)
    {
        var (valido, atualizar) = _passwordHasher.Check(senhaAtual, senhaAnterior);

        return valido;
    }
}
