
using System.Text.Json;
using Operations_Platform_Assignment.Operations;

namespace OperationsPlatform.Tests;

public class AdditionOperationTests
{
    private readonly AdditionOperation _operation = new();

    [Theory]
    [InlineData(5, 7, 12)]
    [InlineData(-5, 3, -2)]
    [InlineData(0, 0, 0)]
    [InlineData(1.5, 2.5, 4)]
    public async Task ExecuteAsync_ValidNumbers_ReturnsCorrectSum(
        double value1,
        double value2,
        double expected)
    {
        // Arrange
        JsonElement parameters = JsonSerializer.SerializeToElement(
            new { value1, value2 });

        // Act
        var result = await _operation.ExecuteAsync(parameters);

        // Assert
        double actual = Assert.IsType<double>(result.Data);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task ExecuteAsync_MissingParameter_ThrowsException()
    {
        // Arrange
        JsonElement parameters = JsonSerializer.SerializeToElement(
            new { value1 = 5 });

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _operation.ExecuteAsync(parameters));
    }

    [Fact]
    public async Task ExecuteAsync_Overflow_ThrowsArgumentException()
    {
        // Arrange
        JsonElement parameters = JsonSerializer.SerializeToElement(
            new { value1 = double.MaxValue, value2 = double.MaxValue });

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _operation.ExecuteAsync(parameters));
    }
}
