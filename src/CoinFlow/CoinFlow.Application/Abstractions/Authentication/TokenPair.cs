namespace CoinFlow.Application.Abstractions.Authentication;

public sealed record TokenPair (
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
