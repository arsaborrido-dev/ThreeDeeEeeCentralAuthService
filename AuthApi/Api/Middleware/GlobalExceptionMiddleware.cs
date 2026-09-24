
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Api.Middleware
{
    using Microsoft.AspNetCore.Mvc;

    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException)
                when (context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Request was cancelled by the client. Path: {Path}",
                    context.Request.Path);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred while processing {Method} {Path}. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);

                if (context.Response.HasStarted)
                {
                    _logger.LogError(
                        "The response has already started, so the error response cannot be written. Path: {Path}, TraceId: {TraceId}",
                        context.Request.Path,
                        context.TraceIdentifier);

                    throw;
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred.",
                Instance = context.Request.Path
            };

            problemDetails.Extensions["traceId"] =
                context.TraceIdentifier;

            context.Response.Clear();

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            context.Response.ContentType =
                "application/problem+json";

            await context.Response.WriteAsJsonAsync(
                problemDetails,
                context.RequestAborted);
        }
    }


    //////public sealed class GlobalExceptionMiddleware
    //////{
    //////    private readonly RequestDelegate _next;
    //////    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    //////    private readonly IHostEnvironment _environment;

    //////    public GlobalExceptionMiddleware(
    //////        RequestDelegate next,
    //////        ILogger<GlobalExceptionMiddleware> logger,
    //////        IHostEnvironment environment)
    //////    {
    //////        _next = next;
    //////        _logger = logger;
    //////        _environment = environment;
    //////    }

    //////    public async Task InvokeAsync(HttpContext context)
    //////    {
    //////        try
    //////        {
    //////            await _next(context);
    //////        }
    //////        catch (OperationCanceledException)
    //////            when (context.RequestAborted.IsCancellationRequested)
    //////        {
    //////            _logger.LogInformation(
    //////                "Request was cancelled by the client. Path: {Path}",
    //////                context.Request.Path);

    //////            // Nothing to return because the client cancelled the request.
    //////        }
    //////        catch (Exception ex)
    //////        {
    //////            _logger.LogError(
    //////                ex,
    //////                "Unhandled exception at {Path}",
    //////                context.Request.Path);

    //////            if (context.Response.HasStarted)
    //////            {
    //////                _logger.LogError(
    //////                    "Cannot write error response because the response has already started. Path: {Path}",
    //////                    context.Request.Path);

    //////                throw;
    //////            }

    //////            await HandleExceptionAsync(context, ex);
    //////        }
    //////    }

    //////    private async Task HandleExceptionAsync(
    //////        HttpContext context,
    //////        Exception exception)
    //////    {
    //////        var problem = CreateProblem(exception, context);

    //////        context.Response.Clear();

    //////        context.Response.StatusCode =
    //////            problem.Status ?? StatusCodes.Status500InternalServerError;

    //////        context.Response.ContentType =
    //////            "application/problem+json";

    //////        await context.Response.WriteAsJsonAsync(
    //////            problem,
    //////            context.RequestAborted);
    //////    }

    //////    private ProblemDetails CreateProblem(
    //////        Exception exception,
    //////        HttpContext context)
    //////    {
    //////        return exception switch
    //////        {
    //////            ProjectNotFoundException =>
    //////                new ProblemDetails
    //////                {
    //////                    Status = StatusCodes.Status404NotFound,
    //////                    Title = "Project Not Found",
    //////                    Detail = "The requested project was not found.",
    //////                    Instance = context.Request.Path
    //////                },

    //////            ValidationException ex =>
    //////                new ProblemDetails
    //////                {
    //////                    Status = StatusCodes.Status400BadRequest,
    //////                    Title = "Bad Request",
    //////                    Detail = ex.Message,
    //////                    Instance = context.Request.Path
    //////                },

    //////            _ =>
    //////                new ProblemDetails
    //////                {
    //////                    Status = StatusCodes.Status500InternalServerError,
    //////                    Title = "Internal Server Error",
    //////                    Detail = _environment.IsDevelopment()
    //////                        ? exception.Message
    //////                        : "An unexpected error occurred.",
    //////                    Instance = context.Request.Path
    //////                }
    //////        };
    //////    }
    //////}

    //public class GlobalExceptionMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    //    private readonly IHostEnvironment _env;

    //    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
    //    {
    //        _next = next;
    //        _logger = logger;
    //        _env = env;
    //    }

    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        try
    //        {
    //            await _next(context);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Unhandled exception at {Path}", context.Request.Path);

    //            var problem = CreateProblem(ex, context);

    //            context.Response.StatusCode = problem.Status ?? 500;
    //            context.Response.ContentType = "application/problem+json";

    //            await context.Response.WriteAsJsonAsync(problem);
    //        }
    //    }

    //    private ProblemDetails CreateProblem(Exception ex, HttpContext context)
    //    {
    //        return ex switch
    //        {
    //            UnauthorizedAccessException => new ProblemDetails
    //            {
    //                Status = (int)HttpStatusCode.Unauthorized,
    //                Title = "Unauthorized",
    //                Detail = "The request is unauthorized.",
    //                //Detail = ex.Message,
    //                Instance = context.Request.Path
    //            },
    //            ArgumentException or ArgumentNullException => new ProblemDetails
    //            {
    //                Status = (int)HttpStatusCode.BadRequest,
    //                Title = "Bad Request",
    //                Detail = "The request was invalid.",
    //                //Detail = ex.Message,
    //                Instance = context.Request.Path
    //            },
    //            KeyNotFoundException => new ProblemDetails
    //            {
    //                Status = (int)HttpStatusCode.NotFound,
    //                Title = "Not Found",
    //                Detail = "The request was not found.",
    //                //Detail = ex.Message,
    //                Instance = context.Request.Path
    //            },
    //            _ => new ProblemDetails
    //            {
    //                Status = (int)HttpStatusCode.InternalServerError,
    //                Title = "Internal Server Error",
    //                Detail = _env.IsDevelopment() ? ex.ToString() : "An unexpected error occurred",
    //                Instance = context.Request.Path
    //            }
    //        };
    //    }
    //}
}