using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Flash.SensitiveWords.Application.Handlers;
using Flash.SensitiveWords.Domain.Interfaces;
using Flash.SensitiveWords.Domain.Services;
using Flash.SensitiveWords.Infrastructure.Data;
using Flash.SensitiveWords.Infrastructure.Repositories;
using Flash.SensitiveWords.API.Middleware;
using Serilog;
using Serilog.Events;

// Configure Serilog for structured logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/api-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    Log.Information("Starting Flash.SensitiveWords API");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Sensitive Words Sanitization API",
            Version = "v1",
            Description = "Microservice for sanitizing sensitive SQL keywords from messages",
            Contact = new OpenApiContact
            {
                Name = "Auratius February",
                Email = "auratius@gmail.com"
            }
        });

        // Tag for monitoring endpoints
        c.TagActionsBy(api =>
        {
            if (api.GroupName != null)
            {
                return new[] { api.GroupName };
            }

            if (api.ActionDescriptor.RouteValues["controller"] != null)
            {
                return new[] { api.ActionDescriptor.RouteValues["controller"] };
            }

            return new[] { "Monitoring" };
        });
    });

    // Add Application Insights telemetry (optional - only if connection string is configured)
    var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    if (!string.IsNullOrEmpty(appInsightsConnectionString))
    {
        builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = appInsightsConnectionString;
        });
        Log.Information("Application Insights enabled");
    }
    else
    {
        Log.Information("Application Insights not configured (add ApplicationInsights:ConnectionString to enable)");
    }

    // Add Health Checks
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddHealthChecks()
        .AddSqlServer(
            connectionString ?? "Server=localhost;Database=SensitiveWordsDb;Integrated Security=true;TrustServerCertificate=true",
            name: "sqlserver",
            timeout: TimeSpan.FromSeconds(3),
            tags: new[] { "db", "sql", "sqlserver" })
        .AddCheck("api", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running"), tags: new[] { "api" });

    builder.Services.AddSingleton<DapperContext>();
    builder.Services.AddScoped<ISensitiveWordRepository, SensitiveWordRepository>();
    builder.Services.AddScoped<ISanitizationService, SanitizationService>();
    builder.Services.AddScoped<IOperationStatsRepository, OperationStatsRepository>();

    builder.Services.AddScoped<GetAllSensitiveWordsHandler>();
    builder.Services.AddScoped<GetSensitiveWordByIdHandler>();
    builder.Services.AddScoped<CreateSensitiveWordHandler>();
    builder.Services.AddScoped<UpdateSensitiveWordHandler>();
    builder.Services.AddScoped<DeleteSensitiveWordHandler>();
    builder.Services.AddScoped<SanitizeMessageHandler>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Add Performance Metrics Middleware
    app.UseMiddleware<PerformanceMetricsMiddleware>();

    // Add Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        };
    });

    // Enable Swagger in all environments (including production for monitoring endpoints)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sensitive Words API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
        c.DocumentTitle = "Sensitive Words API - Swagger UI";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    });

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthorization();
    app.MapControllers();

    // Map Health Check Endpoints with OpenAPI metadata
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    })
    .WithName("GetHealthStatus")
    .WithTags("Monitoring")
    .WithMetadata(new Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute(typeof(object), StatusCodes.Status200OK))
    .WithMetadata(new Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute(StatusCodes.Status503ServiceUnavailable));

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("db"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    })
    .WithName("GetReadinessStatus")
    .WithTags("Monitoring")
    .WithMetadata(new Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute(typeof(object), StatusCodes.Status200OK))
    .WithMetadata(new Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute(StatusCodes.Status503ServiceUnavailable));

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("api"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    })
    .WithName("GetLivenessStatus")
    .WithTags("Monitoring")
    .WithMetadata(new Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute(typeof(object), StatusCodes.Status200OK))
    .WithMetadata(new Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute(StatusCodes.Status503ServiceUnavailable));

    // System metrics endpoint with OpenAPI metadata
    app.MapGet("/metrics", () =>
    {
        var process = System.Diagnostics.Process.GetCurrentProcess();
        return Results.Ok(new
        {
            timestamp = DateTime.UtcNow,
            uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime(),
            memoryUsageMB = process.WorkingSet64 / 1024 / 1024,
            cpuTimeSeconds = process.TotalProcessorTime.TotalSeconds,
            threadCount = process.Threads.Count
        });
    })
    .WithName("GetSystemMetrics")
    .WithTags("Monitoring")
    .WithMetadata(new Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute(typeof(object), StatusCodes.Status200OK))
    .WithSummary("System Metrics")
    .WithDescription("Returns current system metrics including memory usage, CPU time, thread count, and uptime");

    Log.Information("API configured successfully. Starting server...");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

namespace Flash.SensitiveWords.API
{
    // Make the implicit Program class public so integration tests can access it
    public partial class Program { }
}