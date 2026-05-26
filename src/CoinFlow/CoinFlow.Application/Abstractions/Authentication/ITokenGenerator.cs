using CoinFlow.Domain.Users;

namespace CoinFlow.Application.Abstractions.Authentication;

public interface ITokenGenerator
{
    TokenPair Generate(User user);
}
