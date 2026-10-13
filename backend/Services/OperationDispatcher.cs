
using System.Text.Json;
using Operations_Platform_Assignment.Contracts;

namespace Operations_Platform_Assignment.Services;

public sealed class OperationDispatcher
{
    private readonly OperationRegistry _registry;
    private readonly OperationValidator _validator;
    private readonly OperationHistoryService _historyService;

    public OperationDispatcher(
        OperationRegistry registry,
        OperationValidator validator,
        OperationHistoryService historyService)
    {
        _registry = registry;
        _validator = validator;
        _historyService = historyService;
    }

    public async Task<OperationResult?> ExecuteAsync(
        string operationId,
        JsonElement parameters,
        CancellationToken cancellationToken = default)
    {
        var operation = _registry.GetById(operationId);

        if (operation is null)
        {
            return null;
        }

        _validator.Validate(operation, parameters);

        try
        {
            var result = await operation.ExecuteAsync(
                parameters,
                cancellationToken);

            await _historyService.SaveAsync(
                operation.Id,
                parameters.GetRawText(),
                JsonSerializer.Serialize(result.Data),
                "Success",
                null,
                cancellationToken);

            return result;
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _historyService.SaveAsync(
                operation.Id,
                parameters.GetRawText(),
                null,
                "Failed",
                ex.Message,
                cancellationToken);

            throw;
        }
    }
}
