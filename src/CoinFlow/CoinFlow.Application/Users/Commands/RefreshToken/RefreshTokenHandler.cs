using CoinFlow.Application.Abstractions.Authentication;
using CoinFlow.Application.Abstractions.Persistence;
using CoinFlow.Application.Users.Commands.Login;
using CoinFlow.Domain.Exceptions.Users;
using CoinFlow.Domain.Users;
using MediatR;

namespace CoinFlow.Application.Users.Commands.RefreshToken;

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenHandler(
        IUserRepository userRepository,
        ITokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, ct);

        if (user is null)
            throw new InvalidRefreshTokenException();

        var token = user.RefreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);

        if (token is null || !token.IsActive)
            throw new InvalidRefreshTokenException();

        user.RevokeRefreshToken(request.RefreshToken);

        var newTokens = _tokenGenerator.Generate(user);
        user.IssueRefreshToken(newTokens.RefreshToken, newTokens.RefreshTokenExpiresAt);

        _userRepository.Update(user);
        await _unitOfWork.CommitAsync(ct);

        return new LoginResponse
        (
            newTokens.AccessToken,
            newTokens.RefreshToken,
            newTokens.AccessTokenExpiresAt,
            newTokens.RefreshTokenExpiresAt
        );
    }
}
