//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Core.InterfacesOfServices
//{
//    public interface ILoggerMiddlewareService
//    {
//        Task LogRequestResponse(HttpContext httpContext);
//    }
//}
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Core.InterfacesOfServices
{
    public interface ILoggerMiddlewareService
    {
        Task LogRequestResponse(HttpContext httpContext, RequestDelegate next);
    }
}

