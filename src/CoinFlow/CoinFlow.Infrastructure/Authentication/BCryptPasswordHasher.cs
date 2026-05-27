using CoinFlow.Application.Abstractions.Authentication;

namespace CoinFlow.Infrastructure.Authentication;

internal sealed class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12; // Numero de rounds do BCrypt, aumentando o tempo de hash para maior segurança

    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
