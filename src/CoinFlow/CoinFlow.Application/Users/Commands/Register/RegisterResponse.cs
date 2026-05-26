namespace CoinFlow.Application.Users.Commands.Register;

public sealed record RegisterResponse(
    Guid UserId,
    string Email,
    string Name,
    DateTime CreatedAt);
