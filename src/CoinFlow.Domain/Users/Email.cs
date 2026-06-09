using CoinFlow.Domain.Common;
using CoinFlow.Domain.Exceptions.Users;
using System.Text.RegularExpressions;

namespace CoinFlow.Domain.Users;

public class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new InvalidEmailException(input);

        var normalized = input.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new InvalidEmailException(input);

        return new Email(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
