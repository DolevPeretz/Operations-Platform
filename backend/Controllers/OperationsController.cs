
using Microsoft.AspNetCore.Mvc;
using Operations_Platform_Assignment.Contracts;
using Operations_Platform_Assignment.Services;

namespace Operations_Platform_Assignment.Controllers;

[ApiController]
[Route("api/operations")]
public sealed class OperationsController : ControllerBase
{
    private readonly OperationRegistry _registry;
    private readonly OperationDispatcher _dispatcher;
    private readonly OperationHistoryService _historyService;

    public OperationsController(
        OperationRegistry registry,
        OperationDispatcher dispatcher,
        OperationHistoryService historyService)
    {
        _registry = registry;
        _dispatcher = dispatcher;
        _historyService = historyService;
    }

    [HttpGet]
    public IActionResult GetOperations()
    {
        var operations = _registry.GetAll()
            .Select(operation => new
            {
                operation.Id,
                operation.Name,
                operation.Parameters
            });

        return Ok(operations);
    }

    [HttpPost("execute")]
    public async Task<IActionResult> Execute(
        [FromBody] ExecuteOperationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.ExecuteAsync(
            request.OperationId,
            request.Parameters,
            cancellationToken);

        if (result is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Operation not found",
                Detail = $"Operation '{request.OperationId}' does not exist."
            });
        }

        var history = await _historyService.GetHistoryAsync(
            request.OperationId,
            cancellationToken);

        return Ok(new ExecuteOperationResponse(
            result.Data,
            history));
    }
}
