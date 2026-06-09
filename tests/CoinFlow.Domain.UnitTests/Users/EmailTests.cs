using CoinFlow.Domain.Exceptions.Users;
using CoinFlow.Domain.Users;
using Shouldly;

namespace CoinFlow.Domain.UnitTests.Users;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("a@b.c")]
    public void Create_WithInvalidEmail_ShouldReturnEmail(string validEmail)
    {
        var email = Email.Create(validEmail);

        email.Value.ShouldBe(validEmail);
    }

    [Fact]
    public void Create_ShouldNormalizeToLowerCase()
    {
        var email = Email.Create("MATHEUS@CoinFloW.test");

        email.Value.ShouldBe("matheus@coinflow.test");
    }

    [Fact]
    public void Create_ShouldTrimWhitespace()
    {
        var email = Email.Create("    matheus@example.com     ");

        email.Value.ShouldBe("matheus@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("not-an-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@.com")]
    [InlineData("user@example")]
    public void Create_WithInvalidEmail_ShouldThrowInvalidEmailException(string invalidEmail)
    {
        Should.Throw<InvalidEmailException>(() => Email.Create(invalidEmail));
    }

    [Fact]
    public void Create_WithNull_ShouldThrowInvalidEmailException()
    {
        Should.Throw<InvalidEmailException>(() => Email.Create(null!));
    }

    [Fact]
    public void TwoEmails_WithSameValue_ShouldBeEqual()
    {
        var firstEmail = Email.Create("test@example.com");
        var secondEmail = Email.Create("TEST@example.com");

        firstEmail.ShouldBe(secondEmail);
        firstEmail.GetHashCode().ShouldBe(secondEmail.GetHashCode());
    }

    [Fact]
    public void TwoEmails_WithDifferentValues_ShouldNotBeEqual()
    {
        var firstEmail = Email.Create("test1@example.com");
        var secondEmail = Email.Create("test2@example.com");

        firstEmail.ShouldNotBe(secondEmail);
    }
}
