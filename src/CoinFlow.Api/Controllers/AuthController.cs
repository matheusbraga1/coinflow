using CoinFlow.Api.Models.Requests;
using CoinFlow.Api.Models.Responses;
using CoinFlow.Application.Users.Commands.Login;
using CoinFlow.Application.Users.Commands.RefreshToken;
using CoinFlow.Application.Users.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoinFlow.Api.Controllers;

[AllowAnonymous]
public class AuthController : CoinFlowControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [ProducesResponseType(typeof(Models.Responses.RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.Name);
        var result = await _mediator.Send(command, ct);

        var response = new Models.Responses.RegisterResponse(
            result.UserId, 
            result.Email, 
            result.Name, 
            result.CreatedAt);
        return CreatedAtAction(nameof(Register), response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, ct);

        var response = new TokenResponse(
            result.AccessToken,
            result.RefreshToken,
            result.AccessTokenExpiresAt,
            result.RefreshTokenExpiresAt);

        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken ct)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _mediator.Send(command, ct);

        var response = new TokenResponse(
            result.AccessToken,
            result.RefreshToken,
            result.AccessTokenExpiresAt,
            result.RefreshTokenExpiresAt);

        return Ok(response);
    }
}
