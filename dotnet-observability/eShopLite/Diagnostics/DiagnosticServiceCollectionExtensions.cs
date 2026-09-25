using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Azure.Monitor.OpenTelemetry.AspNetCore;

namespace Diagnostics;
public static class DiagnosticServiceCollectionExtensions
{
  public static IServiceCollection AddObservability(this IServiceCollection services,
      string serviceName,
      IConfiguration configuration,
      string[]? meeterNames = null)
  {
    // create the resource that references the service name passed in
    var resource = ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: "1.0");

    // add the OpenTelemetry services
    var otelBuilder = services.AddOpenTelemetry();

    if (!string.IsNullOrEmpty(configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
    {
      otelBuilder.UseAzureMonitor();
    }

    otelBuilder
        // add the metrics providers
        .WithMetrics(metrics =>
        {
          metrics
            .SetResourceBuilder(resource)
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEventCountersInstrumentation(c =>
            {
              c.AddEventSources(
                      "Microsoft.AspNetCore.Hosting",
                      "Microsoft-AspNetCore-Server-Kestrel",
                      "System.Net.Http",
                      "System.Net.Sockets");
            })
            .AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel")
            // .AddConsoleExporter();
            .AddPrometheusExporter();

            // add any additional meters provided by the caller
            if (meeterNames != null)
            {
              foreach (var name in meeterNames)
              {
                metrics.AddMeter(name);
              }
            }

        })
        // add the tracing providers
        .WithTracing(tracing =>
        {
          tracing.SetResourceBuilder(resource)
                      .AddAspNetCoreInstrumentation()
                      .AddHttpClientInstrumentation()
                      .AddSqlClientInstrumentation()
                      .AddZipkinExporter(zipkin =>
                      {
                        var zipkinUrl = configuration["ZIPKIN_URL"] ?? "http://zipkin:9411";
                        zipkin.Endpoint = new Uri($"{zipkinUrl}/api/v2/spans");
                      });
        });

    return services;
  }

  // The prometheus library collects all Metrics to Get /metrics endpoint, 
  // then Prometheus server/container scrapes this endpoint to gather the metrics.
  public static void MapObservability(this IEndpointRouteBuilder routes)
  {
    routes.MapPrometheusScrapingEndpoint();
  }
}
