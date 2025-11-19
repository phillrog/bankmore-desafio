using BankMore.Domain.Core.Commands;

namespace BankMore.Domain.Core.Models
{
    public enum Erro
    {
        INVALID_DOCUMENT,
        USER_UNAUTHORIZED,
        INVALID_ACCOUNT,
        INACTIVE_ACCOUNT,
        INVALID_VALUE,
        INVALID_TYPE,
        INERNAL_ERROR,
        INSUFFICIENT_FUNDS
    }
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public List<string> Erros { get; private set; }
        public string ErroTipo { get; private set; }
        public T Data { get; private set; }

        private Result() { }

        public static Result<T> Success(T data)
        {
            return new Result<T> { IsSuccess = true, Data = data }; 
        }

        public static Result<T> Failure(List<string> errors, Erro? tipo = null)
        {
            return new Result<T> { IsSuccess = false, Erros = errors, ErroTipo = tipo?.ToString() };
        }

        public static Result<T> Failure(string error, Erro? tipo = null)
        {
            return new Result<T> { IsSuccess = false, Erros = new List<string> { error }, ErroTipo = tipo?.ToString() };
        }

        public static Result<T> Failure(Command command, Erro? tipo = null)
        {
            return new Result<T> { IsSuccess = false, Erros = command.ValidationResult.Errors.Select(e => e.ErrorMessage).ToList(), ErroTipo = tipo?.ToString() };
        }

        public static Result<T> Failure(List<string> errors, string tipo = null)
        {
            return new Result<T> { IsSuccess = false, Erros = errors, ErroTipo = tipo?.ToString() };
        }

        public static Result<T> Failure(string error, string tipo = null)
        {
            return new Result<T> { IsSuccess = false, Erros = new List<string> { error }, ErroTipo = tipo?.ToString() };
        }

        public static Result<T> Failure(Command command, string tipo = null)
        {
            return new Result<T> { IsSuccess = false, Erros = command.ValidationResult.Errors.Select(e => e.ErrorMessage).ToList(), ErroTipo = tipo?.ToString() };
        }
    }
}
