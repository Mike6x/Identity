// using System.Net;
// using System.Text.Json;
// using BuildingBlocks.Exceptions;
// using Identity.Core.Exceptions;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Logging;
//
// namespace Identity.Infrastructure.Middleware;
//
// public class ExceptionMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly ILogger<ExceptionMiddleware> _logger;
//
//     public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
//     {
//         _next = next;
//         _logger = logger;
//     }
//
//     public async Task InvokeAsync(HttpContext httpContext)
//     {
//         try
//         {
//             await _next(httpContext);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError($"Something went wrong: {ex}");
//             await HandleExceptionAsync(httpContext, ex);
//         }
//     }
//
//     private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
//     {
//         context.Response.ContentType = "application/json";
//
//         var response = new ErrorResponse
//         {
//             Success = false,
//             Message = "Internal Server Error"
//         };
//
//         switch (exception)
//         {
//             case CustomException ex:
//                 context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//                 response.Message = ex.Message;
//                 response.ErrorCode = ex.ErrorMessages;
//                 break;
//
//             case UnauthorizedAccessException:
//                 context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                 response.Message = "Unauthorized";
//                 break;
//
//             default:
//                 context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                 break;
//         }
//
//         var json = JsonSerializer.Serialize(response);
//         await context.Response.WriteAsync(json);
//     }
// }
