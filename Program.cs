using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace webApi
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        //.Enrich.FromLogContext()
                        //.WriteTo.Console(new RenderedCompactJsonFormatter())
                        //.WriteTo.Console(
                        //        outputTemplate: "[{Timestamp:HH:mm:ss} {CorrelationId} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                        //.WriteTo.EventCollector("", "f53be05e-ff00-44fa-800d-b46eb8fea81b")
                        .CreateLogger();
            CreateHostBuilder(args).Build().Run();

            try
            {
                Log.Information("Getting the motors running...");

                CreateHostBuilder(args).Build().Run();

                return 0;
            }
            catch (Exception ex)
            {   
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
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
