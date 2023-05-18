using Xunit;

namespace PrimeService.Tests;

public class PrimeServiceTest
{
    private readonly PrimeService _primeService = new();

    [Theory]
    [InlineData(-2)]
    [InlineData(0)]
    [InlineData(1)]
    public void IsPrime_ValuesLessThan2_ReturnFalse(int value)
    {
        // Arrange, Act
        var result = _primeService.IsPrime(value);

        // Assert
        Assert.False(result, $"{value} should not be prime");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    public void IsPrime_PrimesLessThan10_ReturnTrue(int value)
    {
        // Arrange, Act
        var result = _primeService.IsPrime(value);

        // Assert
        Assert.True(result, $"{value} should be prime");
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(9)]
    public void IsPrime_NonPrimesLessThan10_ReturnFalse(int value)
    {
        // Arrange, Act
        var result = _primeService.IsPrime(value);

        // Assert
        Assert.False(result, $"{value} should not be prime");
    }
}

public class MathService
{
    public int Multiply(int a, int b) => a * b;
}

public class MathServiceTest
{
    [Fact]
    public void Should_Return_Multiplied_Number()
    {
        // Arrange
        var a = 2;
        var b = 3;
        var service = new MathService();

        // Act
        var result = service.Multiply(a, b);

        // Assert
        Assert.Equal(6, result);
    }
}
