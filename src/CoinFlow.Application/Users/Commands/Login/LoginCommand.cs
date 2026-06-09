using MediatR;

namespace CoinFlow.Application.Users.Commands.Login;

public sealed record LoginCommand (
    string Email,
    string Password) : IRequest<LoginResponse>;
