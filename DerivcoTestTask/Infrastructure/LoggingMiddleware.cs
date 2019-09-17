using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace DerivcoTestTask.Infrastructure
{
    public sealed class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next.Invoke(context);
            await WriteRequestToLog(context.Request);
        }

        private async Task WriteRequestToLog(HttpRequest request)
        {
            var sb = new StringBuilder(1000);
            sb.AppendLine("********************************");
            sb.AppendLine($"Scheme: {request.Scheme}");
            sb.AppendLine($"Host: {request.Host}");
            sb.AppendLine($"Path: {request.Path}");
            sb.AppendLine($"Query: {request.QueryString}");
            sb.AppendLine("********************************");

            await Logger.WriteAsync(sb.ToString());
        }
    }
}
