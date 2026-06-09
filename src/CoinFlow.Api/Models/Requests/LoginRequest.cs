namespace CoinFlow.Api.Models.Requests;

public sealed record LoginRequest(
    string Email,
    string Password);
