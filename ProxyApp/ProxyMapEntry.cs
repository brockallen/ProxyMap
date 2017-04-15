using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace ProxyApp
{
    public class ProxyMapEntry
    {
        public ProxyMapEntry(IApplicationBuilder app, string url)
        {
            var uri = new Uri(url);

            ForwardingPathPrefix = uri.AbsolutePath;
            if (ForwardingPathPrefix == "/") ForwardingPathPrefix = null;

            ProxyRequestDelegate = new Lazy<RequestDelegate>(DelegateFactory(app, uri));
        }

        public string ForwardingPathPrefix { get; set; }
        public Lazy<RequestDelegate> ProxyRequestDelegate { get; set; }

        static Func<RequestDelegate> DelegateFactory(IApplicationBuilder app, Uri uri)
        {
            return () =>
            {
                var builder = app.New();
                builder.RunProxy(new ProxyOptions { Host = uri.Host, Port = uri.Port.ToString(), Scheme = uri.Scheme });
                var proxy = builder.Build();
                return proxy;
            };
        }
    }
}
