using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Splunk;
using Serilog.Formatting.Compact;
using Serilog.Sinks.AzureEventHub;

namespace webApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .WriteTo.Console(new RenderedCompactJsonFormatter())
                            .WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
                            .WriteTo.EventCollector("http://localhost:8088/services/collector", "e8f47278-7ff0-4a33-a370-672afa4879b8")
                            //.WriteTo.AzureEventHub("http://localhost:8088/services/collector", "e8f47278-7ff0-4a33-a370-672afa4879b8")
                            .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
