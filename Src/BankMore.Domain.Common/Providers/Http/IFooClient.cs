using Refit;

namespace BankMore.Domain.Common.Providers.Http;

public interface IFooClient
{
    [Get("/")]
    Task<object> GetAll();
}
