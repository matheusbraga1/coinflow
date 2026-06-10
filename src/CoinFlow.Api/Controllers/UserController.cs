using CoinFlow.Application.Abstractions.Authentication;
using CoinFlow.Application.Users.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoinFlow.Api.Controllers;

[Authorize]
public sealed class UserController : CoinFlowControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public UserController(
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var query = new GetCurrentUserQuery(_currentUserService.UserId);
        var result = await _mediator.Send(query, ct);

        var response = new Models.Responses.CurrentUserResponse(
            result.UserId,
            result.Email,
            result.Name,
            result.CreatedAt);

        return Ok(response);
    }
}
