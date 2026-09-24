using AuthApi.Application.Auth.DTOs;

namespace AuthApi.Application.Common.Models
{

    public record ResponseResult
    {
        public bool Succeeded { get; init; }
        public string? ErrorCode { get; init; }
        public string? ErrorMessage { get; init; }

        public static ResponseResult Success() =>
            new() { Succeeded = true };

        public static ResponseResult Failure(
            string errorCode,
            string errorMessage) =>
            new()
            {
                Succeeded = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
    }

    public record ResponseResult<T> : ResponseResult
    {
        public T? Data { get; init; }
        public static ResponseResult<T> Success(T? data) =>
            new()
            {
                Succeeded = true,
                Data = data
            };

        public static new ResponseResult<T> Failure(
            string errorCode,
            string errorMessage) =>
            new()
            {
                Succeeded = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
    }
}

