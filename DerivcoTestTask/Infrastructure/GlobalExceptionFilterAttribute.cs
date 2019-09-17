using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Text;

namespace DerivcoTestTask.Infrastructure
{
    public sealed class GlobalExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            WriteExceptionToLog(context.Exception);
            context.ExceptionHandled = true;
            var message = "Something went wrong. Try to change your request or write to our support.";
            var response = context.HttpContext.Response;

            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.ContentType = "application/json";
            context.Result = new ObjectResult(new ApiResponse { Message = message, Data = null });
        }

        private void WriteExceptionToLog(Exception exception)
        {
            var sb = new StringBuilder(500);
            sb.AppendLine(exception.Message);
            sb.AppendLine(exception.StackTrace);

            Logger.Write(sb.ToString());
        }
    }

    public sealed class ApiResponse
    {
        public string Message { get; set; }
        public string Data { get; set; }
    }
}