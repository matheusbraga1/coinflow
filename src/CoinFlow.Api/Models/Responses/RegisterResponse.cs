namespace CoinFlow.Api.Models.Responses;

public sealed record RegisterResponse(
    Guid UserId,
    string Email,
    string Name,
    DateTime CreatedAt);
