
namespace Operations_Platform_Assignment.Contracts;

public sealed record OperationHistoryItem(
    Guid Id,
    string OperationId,
    string InputJson,
    string? ResultJson,
    string Status,
    DateTime ExecutedAtUtc
);

public sealed record OperationHistoryResponse(
    IReadOnlyList<OperationHistoryItem> LastThree,
    int MonthlyCount
);

public sealed record ExecuteOperationResponse(
    object? Data,
    OperationHistoryResponse History
);
