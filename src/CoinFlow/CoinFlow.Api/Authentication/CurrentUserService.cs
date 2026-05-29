using CoinFlow.Application.Abstractions.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace CoinFlow.Api.Authentication;

internal sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public Guid UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?
                .User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new InvalidOperationException("Usuário não está autenticado ou sub claim está faltando.");

            return userId;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
