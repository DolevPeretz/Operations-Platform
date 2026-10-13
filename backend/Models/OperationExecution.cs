
namespace Operations_Platform_Assignment.Models;

public sealed class OperationExecution
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string OperationId { get; set; }

    public required string InputJson { get; set; }

    public string? ResultJson { get; set; }

    public required string Status { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime ExecutedAtUtc { get; set; } = DateTime.UtcNow;
}
