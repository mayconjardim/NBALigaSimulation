using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBALigaSimulation.Shared.Models.Utils;
using System.Collections.Generic;

namespace NBALigaSimulation.Server.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma excecao nao tratada. TraceId: {TraceId}", context.TraceIdentifier);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, errorCode, message) = exception switch
            {
                ArgumentException => (
                    StatusCodes.Status400BadRequest,
                    "bad_request",
                    exception.Message),
                KeyNotFoundException => (
                    StatusCodes.Status404NotFound,
                    "not_found",
                    exception.Message),
                UnauthorizedAccessException => (
                    StatusCodes.Status401Unauthorized,
                    "unauthorized",
                    "Acesso nao autorizado."),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "internal_error",
                    "Erro interno do servidor.")
            };

            context.Response.StatusCode = statusCode;

            var response = new ServiceResponse<object>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                TraceId = context.TraceIdentifier
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(json);
        }
    }
}
