namespace BankMore.Domain.ContasCorrentes.Interfaces;

public interface IGeradorNumeroService
{
    public int GerarNumeroConta();
    public string CalcularDigitoVerificador(string numero);

    public string GerarSenha(string senha);
    public bool ValidarSenha(string senha, string salt);
}
