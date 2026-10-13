
using System.Text.Json;
using Operations_Platform_Assignment.Contracts;

namespace Operations_Platform_Assignment.Operations;

public sealed class ReverseTextOperation : IOperation
{
    private static readonly IReadOnlyList<OperationParameter>
        _parameters = Array.AsReadOnly(new[]
        {
            new OperationParameter
            {
                Name = "text",
                Label = "Text to Reverse",
                Type = "text",
                Required = true
            }
        });

    public string Id => "reverse-text";

    public string Name => "Reverse Text";

    public IReadOnlyList<OperationParameter> Parameters
        => _parameters;

    public Task<OperationResult> ExecuteAsync(
        JsonElement parameters,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string text = parameters
            .GetProperty("text")
            .GetString()!;

        string reversed = string.Concat(
            text.EnumerateRunes()
                .Reverse()
                .Select(rune => rune.ToString()));

        return Task.FromResult(
            new OperationResult(reversed));
    }
}
