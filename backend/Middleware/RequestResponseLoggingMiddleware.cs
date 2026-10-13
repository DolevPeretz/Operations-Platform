
using System.Diagnostics;
using System.Text;

namespace Operations_Platform_Assignment.Middleware;

public sealed class RequestResponseLoggingMiddleware
{
    private const int MaxBodyLength = 4096;

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var requestBody = await ReadRequestBodyAsync(context.Request);

        var originalResponseBody = context.Response.Body;

        await using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);

            stopwatch.Stop();

            var responseBody = await ReadResponseBodyAsync(
                responseBodyStream);

            _logger.LogInformation(
                "HTTP Request/Response | Method: {Method} | Path: {Path} | " +
                "RequestBody: {RequestBody} | StatusCode: {StatusCode} | " +
                "ResponseBody: {ResponseBody} | DurationMs: {DurationMs}",
                context.Request.Method,
                context.Request.Path,
                requestBody,
                context.Response.StatusCode,
                responseBody,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "HTTP request failed | Method: {Method} | Path: {Path} | DurationMs: {DurationMs}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        finally
        {
            responseBodyStream.Position = 0;
            context.Response.Body = originalResponseBody;

            await responseBodyStream.CopyToAsync(
                originalResponseBody,
                context.RequestAborted);
        }
    }

    private static async Task<string> ReadRequestBodyAsync(
        HttpRequest request)
    {
        if (!IsJsonContent(request.ContentType))
        {
            return "[Non-JSON request]";
        }

        request.EnableBuffering();

        try
        {
            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            var buffer = new char[MaxBodyLength + 1];

            var charactersRead = await reader.ReadBlockAsync(
                buffer.AsMemory(),
                request.HttpContext.RequestAborted);

            var body = new string(
                buffer,
                0,
                Math.Min(charactersRead, MaxBodyLength));

            return charactersRead > MaxBodyLength
                ? body + "...[Truncated]"
                : body;
        }
        finally
        {
            request.Body.Position = 0;
        }
    }

    private static async Task<string> ReadResponseBodyAsync(
        MemoryStream responseStream)
    {
        responseStream.Position = 0;

        using var reader = new StreamReader(
            responseStream,
            Encoding.UTF8,
            leaveOpen: true);

        var buffer = new char[MaxBodyLength + 1];

        var charactersRead = await reader.ReadBlockAsync(
            buffer.AsMemory());

        responseStream.Position = 0;

        var body = new string(
            buffer,
            0,
            Math.Min(charactersRead, MaxBodyLength));

        return charactersRead > MaxBodyLength
            ? body + "...[Truncated]"
            : body;
    }

    private static bool IsJsonContent(string? contentType)
    {
        return contentType is not null &&
            (contentType.Contains(
                "application/json",
                StringComparison.OrdinalIgnoreCase) ||
             contentType.Contains(
                "+json",
                StringComparison.OrdinalIgnoreCase));
    }
}
