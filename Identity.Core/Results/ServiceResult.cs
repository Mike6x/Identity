// namespace Identity.API.Results;
//
// public class ServiceResult<T>
// {
//     public bool Succeeded { get; set; }
//     public string Message { get; set; }
//     public string ErrorCode { get; set; }
//     public T Data { get; set; }
//     public IDictionary<string, string[]> Errors { get; set; }
//
//     public static ServiceResult<T> Success(T data, string message = null)
//     {
//         return new ServiceResult<T>
//         {
//             Succeeded = true,
//             Data = data,
//             Message = message
//         };
//     }
//
//     public static ServiceResult<T> Failure(string errorMessage, string errorCode = null, IDictionary<string, string[]> errors = null)
//     {
//         return new ServiceResult<T>
//         {
//             Succeeded = false,
//             Message = errorMessage,
//             ErrorCode = errorCode,
//             Errors = errors,
//         };
//     }
// }
