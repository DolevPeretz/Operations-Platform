
using System.Text.Json;
using Operations_Platform_Assignment.Contracts;

namespace Operations_Platform_Assignment.Operations;

public interface IOperation
{
    string Id { get; }

    string Name { get; }

    IReadOnlyList<OperationParameter> Parameters { get; }

    Task<OperationResult> ExecuteAsync(
        JsonElement parameters,
        CancellationToken cancellationToken = default
    );
}
