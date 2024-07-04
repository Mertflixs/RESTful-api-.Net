using FluentValidation.Results;

namespace Ppr.Rest.Api {
    //Bu sınıf ile ApiResponsumuza gelen verilere göre istediğimiz formatta dönüş değerleri alamayı sağlıyoruz
    public class ApiResponse<T> {
        public T Data { get; set; }
        public string Error { get; set; }
        public bool IsSuccess { get; set; }

        public ApiResponse(T data) {
            this.Data = data;
            this.IsSuccess = true;
            this.Error = string.Empty;
        }

        public ApiResponse(string message) {
            this.Data = default!;
            this.IsSuccess = false;
            this.Error = message;
        }
    }

    public static class ApiResponseExtensions {
        //FluentValidation gelen hatalar burada istediğimiz response şekline çevriliyor ve dönen hata kodları ile responselarımızı görüyoruz
        public static ApiResponse<T> ToApiResponse<T>(this ValidationResult validationResult) {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            var errorMessage = string.Join("; ", errors);
            return new ApiResponse<T>(errorMessage);
        }
    }
}