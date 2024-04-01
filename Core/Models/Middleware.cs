using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

public class Middleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<Middleware> _logger;

    private int requestIdCounter = 1; // Initialize a counter for request Ids

    public Middleware(RequestDelegate next, ILogger<Middleware> logger)
    {
        _next = next;
        _logger = logger;        
    }

    public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
    {
        // Buffer the request body in order to read it
        context.Request.EnableBuffering();

        // Read the request body
        var requestBody = await ReadRequestBody(context.Request);

        var maxId = await dbContext.Logs.MaxAsync(l => (int?)l.Id) ?? 0;
        var requestId = maxId + 1;

        // Log the request
        var requestLog = new Logs
        {
            Id = requestId,
            Timestamp = DateTime.UtcNow,
            Method = context.Request.Method,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            Headers = FormatHeaders(context.Request.Headers),
            RequestBody = requestBody,
            StatusCode = 0, // Placeholder value, will be updated in LogResponse
            ContentType = context.Request.ContentType,
            ResponseBody = null // Placeholder value, will be updated in LogResponse
        };

        dbContext.Logs.Add(requestLog);
        await dbContext.SaveChangesAsync();

        // Rewind the request body to enable subsequent reads by other components
        context.Request.Body.Position = 0;

        // Replace the response body with a MemoryStream to intercept it
        using (var responseBody = new MemoryStream())
        {
            var originalResponseBody = context.Response.Body;
            context.Response.Body = responseBody;

            // Call the next middleware in the pipeline
            await _next(context);

            // Read the response body
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseBodyContent = await new StreamReader(responseBody).ReadToEndAsync();

            // Log the response
            var responseLog = await dbContext.Logs.OrderByDescending(l => l.Timestamp).FirstOrDefaultAsync();
            if (responseLog != null)
            {
                responseLog.StatusCode = context.Response.StatusCode;
                responseLog.ResponseBody = responseBodyContent;
                await dbContext.SaveChangesAsync();
            }

            // Copy the response body back to the original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalResponseBody);



        }
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
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
}
