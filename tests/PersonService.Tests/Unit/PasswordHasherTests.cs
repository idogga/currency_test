using PersonService.Common.Services;

namespace PersonService.Tests.Unit;

/// <summary>
/// Проверяет хэширование паролей и проверку их соответствия хэшу.
/// </summary>
public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher = new();

    [Fact]
    public void Hash_ReturnsHashThatVerifiesOriginalPassword()
    {
        const string password = "correct horse battery staple";

        var hash = _passwordHasher.Hash(password);

        Assert.NotEqual(password, hash);
        Assert.True(_passwordHasher.Verify(password, hash));
    }

    [Fact]
    public void Verify_ReturnsFalseForDifferentPassword()
    {
        var hash = _passwordHasher.Hash("correct horse battery staple");

        var result = _passwordHasher.Verify("different password", hash);

        Assert.False(result);
    }
}