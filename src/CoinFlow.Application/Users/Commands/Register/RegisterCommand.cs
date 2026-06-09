using MediatR;

namespace CoinFlow.Application.Users.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string Name) : IRequest<RegisterResponse>;
