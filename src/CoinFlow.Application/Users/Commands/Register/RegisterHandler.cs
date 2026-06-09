using CoinFlow.Application.Abstractions.Authentication;
using CoinFlow.Application.Abstractions.Persistence;
using CoinFlow.Application.Users.Exceptions;
using CoinFlow.Domain.Users;
using MediatR;

namespace CoinFlow.Application.Users.Commands.Register;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken ct)
    {
        var email = Email.Create(request.Email);

        if (await _userRepository.EmailExistsAsync(email, ct))
            throw new EmailAlreadyExistsException(email.Value);

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(email, passwordHash, request.Name);

        await _userRepository.AddAsync(user, ct);
        await _unitOfWork.CommitAsync(ct);

        return new RegisterResponse
        (
            user.Id,
            user.Email.Value,
            user.Name,
            user.CreatedAt
        );
    }
}
