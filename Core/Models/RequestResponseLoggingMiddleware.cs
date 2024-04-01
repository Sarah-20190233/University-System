using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var requestBody = await ReadRequestBody(context.Request);
                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    try
                    {
                        await _next(context);

                        var response = await FormatResponse(context.Response);

                        var log = new Logs
                        {
                            Timestamp = DateTime.UtcNow,
                            Method = context.Request.Method,
                            Path = context.Request.Path,
                            QueryString = context.Request.QueryString.ToString(),
                            Headers = FormatHeaders(context.Request.Headers),
                            RequestBody = requestBody,
                            StatusCode = context.Response.StatusCode,
                            ContentType = context.Response.ContentType,
                            ResponseBody = response
                        };

                        await LogRequestResponse(dbContext, log);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle any exceptions that occur during logging
                        Console.WriteLine($"Error logging request/response: {ex.Message}");
                    }
                    finally
                    {
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var (key, value) in headers)
            {
                builder.Append($"{key}: {string.Join(",", value)}\n");
            }
            return builder.ToString();
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using (var memoryStream = new MemoryStream())
            {
                await response.Body.CopyToAsync(memoryStream);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        private async Task LogRequestResponse(ApplicationDbContext dbContext, Logs log)
        {
            try
            {
                dbContext.Logs.Add(log);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log or handle any exceptions that occur during logging
                Console.WriteLine($"Error logging request/response: {ex.Message}");
            }
        }
    }
}
