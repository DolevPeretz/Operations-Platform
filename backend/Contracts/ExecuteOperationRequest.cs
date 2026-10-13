
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Operations_Platform_Assignment.Contracts;

public sealed class ExecuteOperationRequest
{
    [Required]
    public required string OperationId { get; init; }

    [Required]
    public JsonElement Parameters { get; init; }
}
