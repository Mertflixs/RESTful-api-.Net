using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Ppr.Rest.Api.Base {
    public class ErrorHandlerMiddleware {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next) {
            _next = next;
        }

        //Middleware i Invoke komutu ile HTTP isteklerini işler 
        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);
            } catch (Exception error) {
                var response = context.Response;
                response.ContentType = "application/json";

                var responseModel = new ApiResponse<string>(error?.Message);

                //Hata koduna göre uygun HTTP durumu kodunu ayarlar
                switch (error) {
                    case KeyNotFoundException e:
                        //Kaynak bulunamaz ise 404 Not Found yanıtını verir
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case ArgumentException e:
                        //Kötü istek durumunda 400 Bad Request yanıtını verir
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    default:
                        //Diğer tüm durumlar için 500 Internal Server Error yanıtını verir
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                //ApiResponse nesnesini JSON formatına dönüştürür ve HTTP yanıt gövdesine işler
                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}