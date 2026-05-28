namespace CoinFlow.Api.Models.Requests;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string Name);
