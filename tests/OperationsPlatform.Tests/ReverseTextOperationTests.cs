
using System.Text.Json;
using Operations_Platform_Assignment.Operations;

namespace OperationsPlatform.Tests;

public class ReverseTextOperationTests
{
    private readonly ReverseTextOperation _operation = new();

    [Theory]
    [InlineData("hello", "olleh")]
    [InlineData("abc", "cba")]
    [InlineData("12345", "54321")]
    [InlineData("a", "a")]
    [InlineData("", "")]
    public async Task ExecuteAsync_ValidText_ReturnsReversedText(
        string input,
        string expected)
    {
        // Arrange
        JsonElement parameters = JsonSerializer.SerializeToElement(
            new { text = input });

        // Act
        var result = await _operation.ExecuteAsync(parameters);

        // Assert
        string actual = Assert.IsType<string>(result.Data);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task ExecuteAsync_UnicodeText_ReversesCorrectly()
    {
        // Arrange
        JsonElement parameters = JsonSerializer.SerializeToElement(
            new { text = "A😀B" });

        // Act
        var result = await _operation.ExecuteAsync(parameters);

        // Assert
        string actual = Assert.IsType<string>(result.Data);
        Assert.Equal("B😀A", actual);
    }

    [Fact]
    public async Task ExecuteAsync_MissingText_ThrowsException()
    {
        // Arrange
        JsonElement parameters = JsonSerializer.SerializeToElement(
            new { });

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _operation.ExecuteAsync(parameters));
    }
}
