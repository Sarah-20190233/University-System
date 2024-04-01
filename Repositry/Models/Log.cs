using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Log
{
    public int Id { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Method { get; set; }

    public string? Path { get; set; }

    public string? QueryString { get; set; }

    public string? Headers { get; set; }

    public string? RequestBody { get; set; }

    public int? StatusCode { get; set; }

    public string? ContentType { get; set; }

    public string? ResponseBody { get; set; }
}
