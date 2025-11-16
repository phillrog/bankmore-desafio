namespace BankMore.Domain.Common.Providers.Hash;

public interface IPasswordHasher
{
    (string senha, string salt) Hash(string password);

    (bool Verified, bool NeedsUpgrade) Check(string hash, string password);
}
