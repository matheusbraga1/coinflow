using CoinFlow.Api.Models.Requests;
using CoinFlow.Api.Models.Responses;
using CoinFlow.Application.Users.Commands.Login;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace CoinFlow.Integration.Tests.Auth;

public class AuthEndpointsTests : IClassFixture<CoinFlowApiFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CoinFlowApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Register_WithValidData_ShouldReturnCreated()
    {
        var request = new RegisterRequest(
            "matheus@coinflow.test",
            "Senha@1234",
            "Matheus");

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            request,
            CancellationToken.None);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>(CancellationToken.None);
        body.ShouldNotBeNull();
        body!.Email.ShouldBe(request.Email);
    }

    [Fact]
    public async Task Register_WithInvalidPassword_ShouldReturnBadRequest()
    {
        var request = new RegisterRequest(
            "matheus@coinflow.test",
            "123",
            "Matheus");

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            request,
            CancellationToken.None);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    // TODO: Refact this test to separate the register and login steps, and add more assertions to validate the responses
    [Fact]
    public async Task FullFlow_RegisterLoginAccessProtectedEndpoint_ShouldWork()
    {
        var email = "matheus@coinflow.test";

        var resgisterRequest = new RegisterRequest(
            email,
            "Senha@1234",
            "Matheus");
        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            resgisterRequest,
            CancellationToken.None);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var loginRequest = new LoginRequest(email, "Senha@1234");
        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            loginRequest,
            CancellationToken.None);
        loginResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tokens = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(CancellationToken.None);
        tokens.ShouldNotBeNull();
        tokens!.AccessToken.ShouldNotBeNullOrWhiteSpace();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        var meResponse = await _client.GetAsync("/api/user/me", CancellationToken.None);
        meResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var me = await meResponse.Content.ReadFromJsonAsync<CurrentUserResponse>(CancellationToken.None);
        me.ShouldNotBeNull();
        me!.Email.ShouldBe(email);
    }

    [Fact]
    public async Task GetMe_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/user/me", CancellationToken.None);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
