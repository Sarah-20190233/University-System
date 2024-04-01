using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Logs
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public DateTime Timestamp { get; set; }

        public string Method { get; set; }

        public string Path { get; set; }

        public string QueryString { get; set; }

        public string Headers { get; set; }

        public string RequestBody { get; set; }

        public int StatusCode { get; set; }

        public string ContentType { get; set; }

        public string ResponseBody { get; set; }

    }
}
