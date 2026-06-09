using CoinFlow.Application.Users.Commands.Login;
using MediatR;

namespace CoinFlow.Application.Users.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResponse>;
