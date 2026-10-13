
using System.Text.Json;
using Operations_Platform_Assignment.Contracts;

namespace Operations_Platform_Assignment.Operations;

public sealed class AdditionOperation : IOperation
{
    private static readonly IReadOnlyList<OperationParameter>
        _parameters = Array.AsReadOnly(new[]
        {
            new OperationParameter
            {
                Name = "value1",
                Label = "First Number",
                Type = "number"
            },
            new OperationParameter
            {
                Name = "value2",
                Label = "Second Number",
                Type = "number"
            }
        });

    public string Id => "addition";

    public string Name => "Addition";

    public IReadOnlyList<OperationParameter> Parameters => _parameters;

    public Task<OperationResult> ExecuteAsync(
        JsonElement parameters,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        double value1 = parameters.GetProperty("value1").GetDouble();
        double value2 = parameters.GetProperty("value2").GetDouble();

        double sum = value1 + value2;

        if (!double.IsFinite(sum))
        {
            throw new ArgumentException(
                "The calculation result must be a finite number.");
        }

        return Task.FromResult(new OperationResult(sum));
    }
}
