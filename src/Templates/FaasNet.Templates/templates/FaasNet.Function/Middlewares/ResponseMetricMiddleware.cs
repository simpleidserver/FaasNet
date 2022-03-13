using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using FaasNet.Function;

namespace Function.Middlewares
{
    public class ResponseMetricMiddleware
    {
        private readonly RequestDelegate _request;

        public ResponseMetricMiddleware(RequestDelegate request)
        {
            _request = request;
        }

        public async Task Invoke(HttpContext httpContext, MetricReporter reporter)
        {
            var path = httpContext.Request.Path.Value;
            if (httpContext.Request.Method != HttpMethods.Post)
            {
                await _request.Invoke(httpContext);
                return;
            }


            var sw = Stopwatch.StartNew();
            try
            {
                await _request.Invoke(httpContext);
            }
            finally
            {
                sw.Stop();
                reporter.RegisterRequest();
                reporter.RegisterResponseTime(httpContext.Response.StatusCode, httpContext.Request.Method, sw.Elapsed);
            }
        }
    }
}
