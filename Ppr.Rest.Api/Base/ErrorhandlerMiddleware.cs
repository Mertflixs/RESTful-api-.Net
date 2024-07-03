using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Ppr.Rest.Api.Base {
    public class ErrorHandlerMiddleware {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);
            } catch (Exception error) {
                var response = context.Response;
                response.ContentType = "application/json";

                var responseModel = new ApiResponse<string>(error?.Message);

                switch (error) {
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case ArgumentException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}