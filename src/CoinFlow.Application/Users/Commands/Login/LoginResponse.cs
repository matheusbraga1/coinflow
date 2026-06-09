namespace CoinFlow.Application.Users.Commands.Login;

public sealed record LoginResponse (
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
