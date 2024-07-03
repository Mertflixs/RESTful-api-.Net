using FluentValidation.Results;

namespace Ppr.Rest.Api {
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
        public static ApiResponse<T> ToApiResponse<T>(this ValidationResult validationResult) {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            var errorMessage = string.Join("; ", errors);
            return new ApiResponse<T>(errorMessage);
        }
    }
}