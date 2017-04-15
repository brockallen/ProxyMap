using System.Collections.Generic;
using ProxyApp;

namespace Microsoft.AspNetCore.Builder
{
    public static class ProxyMapMiddlewareExtensions
    {
        public static IApplicationBuilder UseProxyMap(this IApplicationBuilder app, Dictionary<string, string> map)
        {
            return app.UseMiddleware<ProxyMapMiddleware>(app, map);
        }
    }
}
