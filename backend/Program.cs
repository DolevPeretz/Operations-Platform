
using Microsoft.EntityFrameworkCore;
using Operations_Platform_Assignment.Data;
using Operations_Platform_Assignment.Exceptions;
using Operations_Platform_Assignment.Middleware;
using Operations_Platform_Assignment.Operations;
using Operations_Platform_Assignment.Services;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Logger(logger => logger
        .Filter.ByIncludingOnly(logEvent =>
            logEvent.Level >= LogEventLevel.Error)
        .WriteTo.File(
            "logs/errors-.log",
            rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(logger => logger
        .Filter.ByIncludingOnly(
            Matching.FromSource<RequestResponseLoggingMiddleware>())
        .WriteTo.File(
            "logs/requests-.log",
            rollingInterval: RollingInterval.Day))
    .CreateLogger();

builder.Host.UseSerilog();

// Controllers
builder.Services.AddControllers();

// Global Exception Handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClient", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Weather API
builder.Services.AddHttpClient<WeatherService>(client =>
{
    client.BaseAddress = new Uri("https://api.open-meteo.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Operations
builder.Services.AddTransient<IOperation, AdditionOperation>();
builder.Services.AddTransient<IOperation, ReverseTextOperation>();
builder.Services.AddTransient<IOperation, WeatherOperation>();

// Services
builder.Services.AddScoped<OperationRegistry>();
builder.Services.AddScoped<OperationValidator>();
builder.Services.AddScoped<OperationDispatcher>();
builder.Services.AddScoped<OperationHistoryService>();

var app = builder.Build();

// Middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactClient");
app.UseAuthorization();

app.MapControllers();

app.Run();
