using CoinFlow.Application.Abstractions.Authentication;
using CoinFlow.Application.Abstractions.Persistence;
using CoinFlow.Domain.Exceptions.Users;
using CoinFlow.Domain.Users;
using MediatR;

namespace CoinFlow.Application.Users.Commands.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var email = Email.Create(request.Email);
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var tokens = _tokenGenerator.Generate(user);
        user.IssueRefreshToken(tokens.RefreshToken, tokens.RefreshTokenExpiresAt);

        _userRepository.Update(user);
        await _unitOfWork.CommitAsync(ct);

        return new LoginResponse(
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshTokenExpiresAt);
    }
}
