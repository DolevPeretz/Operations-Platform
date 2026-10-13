
using Operations_Platform_Assignment.Contracts;
using Microsoft.EntityFrameworkCore;
using Operations_Platform_Assignment.Contracts;
using Operations_Platform_Assignment.Data;
using Operations_Platform_Assignment.Models;

namespace Operations_Platform_Assignment.Services;

public sealed class OperationHistoryService
{
    private readonly AppDbContext _dbContext;

    public OperationHistoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(
        string operationId,
        string inputJson,
        string? resultJson,
        string status,
        string? errorMessage,
        CancellationToken cancellationToken = default)
    {
        var execution = new OperationExecution
        {
            OperationId = operationId,
            InputJson = inputJson,
            ResultJson = resultJson,
            Status = status,
            ErrorMessage = errorMessage,
            ExecutedAtUtc = DateTime.UtcNow
        };

        _dbContext.OperationExecutions.Add(execution);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<OperationExecution>> GetLastThreeAsync(
        string operationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.OperationExecutions
            .AsNoTracking()
            .Where(e => e.OperationId == operationId)
            .OrderByDescending(e => e.ExecutedAtUtc)
            .ThenByDescending(e => e.Id)
            .Take(3)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetMonthlyCountAsync(
        string operationId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var startOfMonth = new DateTime(
            now.Year,
            now.Month,
            1,
            0,
            0,
            0,
            DateTimeKind.Utc);

        return await _dbContext.OperationExecutions
            .AsNoTracking()
            .CountAsync(
                e => e.OperationId == operationId &&
                     e.ExecutedAtUtc >= startOfMonth &&
                     e.ExecutedAtUtc <= now,
                cancellationToken);
    }


    public async Task<OperationHistoryResponse> GetHistoryAsync(
        string operationId,
        CancellationToken cancellationToken = default)
    {
        var lastThree = await GetLastThreeAsync(
            operationId,
            cancellationToken);

        var monthlyCount = await GetMonthlyCountAsync(
            operationId,
            cancellationToken);

        var historyItems = lastThree
            .Select(e => new OperationHistoryItem(
                e.Id,
                e.OperationId,
                e.InputJson,
                e.ResultJson,
                e.Status,
                e.ExecutedAtUtc))
            .ToList();

        return new OperationHistoryResponse(
            historyItems,
            monthlyCount);
    }

}
