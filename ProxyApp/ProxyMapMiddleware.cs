using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace ProxyApp
{
    public class ProxyMapMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<PathString, ProxyMapEntry> _map = new Dictionary<PathString, ProxyMapEntry>();

        public ProxyMapMiddleware(RequestDelegate next, IApplicationBuilder app, Dictionary<string, string> map)
        {
            _next = next;

            foreach (var item in map)
            {
                _map.Add(item.Key, new ProxyMapEntry(app, item.Value));
            }
        }

        public async Task Invoke(HttpContext context)
        {
            var requestPath = context.Request.Path;
            foreach(var item in _map)
            {
                if (requestPath.StartsWithSegments(item.Key, out var remaining))
                {
                    context.Request.Path = item.Value.ForwardingPathPrefix + remaining;
                    await item.Value.ProxyRequestDelegate.Value(context);
                    return;
                }
                else if (item.Key == "/")
                {
                    context.Request.Path = item.Value.ForwardingPathPrefix + context.Request.Path;
                    await item.Value.ProxyRequestDelegate.Value(context);
                    return;
                }
            }

            await _next(context);
        }
    }
}
