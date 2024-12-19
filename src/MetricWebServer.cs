using EmbedIO;
using EmbedIO.Actions;
using Prometheus;
using System;
using System.Threading.Tasks;

namespace AlexHelms.NINA.Prometheusexporter;

public class MetricWebServer(int Port) : IDisposable
{
    private WebServer _webServer;

    public Task Start()
    {
        _webServer = new WebServer(o => o
                .WithUrlPrefix($"http://*:{Port}")
                .WithMode(HttpListenerMode.EmbedIO))
            .WithModule(new ActionModule("/metrics", HttpVerbs.Get, async ctx =>
            {
                ctx.Response.ContentType = "text/plain";
                using var stream = ctx.OpenResponseStream();
                await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream, ctx.CancellationToken);
            })
        );

        _webServer.Start();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _webServer?.Dispose();
        _webServer = null;
    }
}
