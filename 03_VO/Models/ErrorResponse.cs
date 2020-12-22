using System;

namespace APIRest_ASPNET5.Models
{
    public class ErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public ErrorResponse InnerError { get; set; }
        public static ErrorResponse From(Exception e)
        {
            if (e == null)
            {
                return null;
            }
            return new ErrorResponse
            {
                Code = e.HResult,
                Message = e.Message,
                InnerError = ErrorResponse.From(e.InnerException)
            };
        }
    }
}