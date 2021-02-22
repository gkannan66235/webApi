## Looging Framework:

For .Net structured logging we have top 3 frameworks log4net, nlog and serilog.
- log4net  — battle tested, in system for longtime and 2x growth in usage (nuget download)
- nlog — simple to use, later added support for structured logging, 2x growth in usage
- serilog — simple, built for structured logging and supports out of the box, default json output format, 3x growth in usage
* NuGet package statistics for the [last 6 weeks](https://www.nuget.org/stats/packages)![Screenshot 2021-02-22 at 2.52.27 PM.png](/.attachments/Screenshot%202021-02-22%20at%202.52.27%20PM-1ebe7729-3c09-4611-a8c0-6c992c0b8a17.png)
* Sinks available - log4net (lots of custom appenders), nlog-93, serilog-94

What other research says: 
- [Developer Tips](https://stackify.com/nlog-vs-log4net-vs-serilog/)
- [Aspects of ease of use, log speed, and missing logs per total logs](https://medium.com/dev-genius/serilog-vs-nlog-7d0a322a4732)
- [With elmah](https://blog.elmah.io/serilog-vs-log4net/#:~:text=log4net%20doesn't%20support%20the,Serilog%20example%20isn't%20available.)
- [User reactions](https://www.slant.co/topics/61/versus/~nlog_vs_serilog_vs_log4net)
![Screenshot 2021-02-22 at 2.57.32 PM.png](/.attachments/Screenshot%202021-02-22%20at%202.57.32%20PM-904ee4a8-eb25-468a-ad25-7eb722c2ab68.png)

**Conclusion:** Based on user research and developers feedback serilog taken as logging tool.

## Decision on log forwarder:
* Logging destination : Splunk Cloud
* Output : Console, WebApp logging system will read console logs and forwards to Splunk -- Need ADR ??

## For configuration:

1. Provide logging configuration through file (appsettings.json) using [serilog-settings-configuration](https://github.com/serilog/serilog-settings-configuration) for different application/env.
```
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console" ],
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "formatter": "Serilog.Formatting.Compact.RenderedCompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ],
    "Enrich": [ "FromLogContext" ],
    "Properties": {
      "Application": "SampleApi"
    }
  }
}
```

2. Need to create Standard Logging library to abstract logging standards -- Create and add this package to nuget package manager (Probably Azure Artifacts) -- Card in backlog


## For development: 
1. Install the Serilog.AspNetCore NuGet package into your app. Ref: https://github.com/serilog/serilog-aspnetcore
2. Application should not fail if the logger fails and it should write the event to debug console.
3. Write log information 
Ex: `_logger.LogInformation("called claim {CliamId}", CliamId);`
4. Use ForContext() method returns an ILogger that attaches a specified property to all of the events logged through it.
Ex: 
```
_logger
        .ForContext("ClaimId", claimId)
        .LogInformation("called claim id") 
        .LogInformation("Storing claim state in DB");`
```
5. Logging configuration and output.
Using output Template:
```
.WriteTo.Console(new RenderedCompactJsonFormatter())
```                                    
Log Output:
```

{"@t":"2021-02-18T17:35:24.0909180Z","@m":"Executing endpoint '\"webApi.Controllers.WeatherForecastController.Get (webApi)\"'","@i":"500cc934","EndpointName":"webApi.Controllers.WeatherForecastController.Get (webApi)","EventId":{"Name":"ExecutingEndpoint"},"SourceContext":"Microsoft.AspNetCore.Routing.EndpointMiddleware","RequestId":"0HM6K6E8THHIM:00000001","RequestPath":"/weatherforecast","SpanId":"|f9b5b47-41bb05900fe0ec5d.","TraceId":"f9b5b47-41bb05900fe0ec5d","ParentId":"","ConnectionId":"0HM6K6E8THHIM"}
{"@t":"2021-02-18T17:35:24.0913630Z","@m":"Route matched with \"{action = \\\"Get\\\", controller = \\\"WeatherForecast\\\"}\". Executing controller action with signature \"System.Collections.Generic.IEnumerable`1[webApi.WeatherForecast] Get()\" on controller \"webApi.Controllers.WeatherForecastController\" (\"webApi\").","@i":"122b2fdf","RouteData":"{action = \"Get\", controller = \"WeatherForecast\"}","MethodInfo":"System.Collections.Generic.IEnumerable`1[webApi.WeatherForecast] Get()","Controller":"webApi.Controllers.WeatherForecastController","AssemblyName":"webApi","EventId":{"Id":3,"Name":"ControllerActionExecuting"},"SourceContext":"Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker","ActionId":"f67fe2df-841e-4087-bee1-c62c29dda8d9","ActionName":"webApi.Controllers.WeatherForecastController.Get (webApi)","RequestId":"0HM6K6E8THHIM:00000001","RequestPath":"/weatherforecast","SpanId":"|f9b5b47-41bb05900fe0ec5d.","TraceId":"f9b5b47-41bb05900fe0ec5d","ParentId":"","ConnectionId":"0HM6K6E8THHIM"}
{"@t":"2021-02-18T17:35:24.0918260Z","@m":"called claim \"az500\"","@i":"8f2a5d4a","ClaimId":"az500","SourceContext":"webApi.Controllers.WeatherForecastController","ActionId":"f67fe2df-841e-4087-bee1-c62c29dda8d9","ActionName":"webApi.Controllers.WeatherForecastController.Get (webApi)","RequestId":"0HM6K6E8THHIM:00000001","RequestPath":"/weatherforecast","SpanId":"|f9b5b47-41bb05900fe0ec5d.","TraceId":"f9b5b47-41bb05900fe0ec5d","ParentId":"","ConnectionId":"0HM6K6E8THHIM"}
{"@t":"2021-02-18T17:35:24.0926070Z","@m":"Executing ObjectResult, writing value of type '\"webApi.WeatherForecast[]\"'.","@i":"8a1b66c8","Type":"webApi.WeatherForecast[]","EventId":{"Id":1,"Name":"ObjectResultExecuting"},"SourceContext":"Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor","ActionId":"f67fe2df-841e-4087-bee1-c62c29dda8d9","ActionName":"webApi.Controllers.WeatherForecastController.Get (webApi)","RequestId":"0HM6K6E8THHIM:00000001","RequestPath":"/weatherforecast","SpanId":"|f9b5b47-41bb05900fe0ec5d.","TraceId":"f9b5b47-41bb05900fe0ec5d","ParentId":"","ConnectionId":"0HM6K6E8THHIM"}
{"@t":"2021-02-18T17:35:24.0933670Z","@m":"Executed action \"webApi.Controllers.WeatherForecastController.Get (webApi)\" in 1.8193ms","@i":"afa2e885","ActionName":"webApi.Controllers.WeatherForecastController.Get (webApi)","ElapsedMilliseconds":1.8193,"EventId":{"Id":2,"Name":"ActionExecuted"},"SourceContext":"Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker","ActionId":"f67fe2df-841e-4087-bee1-c62c29dda8d9","RequestId":"0HM6K6E8THHIM:00000001","RequestPath":"/weatherforecast","SpanId":"|f9b5b47-41bb05900fe0ec5d.","TraceId":"f9b5b47-41bb05900fe0ec5d","ParentId":"","ConnectionId":"0HM6K6E8THHIM"}
{"@t":"2021-02-18T17:35:24.0935440Z","@m":"Executed endpoint '\"webApi.Controllers.WeatherForecastController.Get (webApi)\"'","@i":"99874f2b","EndpointName":"webApi.Controllers.WeatherForecastController.Get (webApi)","EventId":{"Id":1,"Name":"ExecutedEndpoint"},"SourceContext":"Microsoft.AspNetCore.Routing.EndpointMiddleware","RequestId":"0HM6K6E8THHIM:00000001","RequestPath":"/weatherforecast","SpanId":"|f9b5b47-41bb05900fe0ec5d.","TraceId":"f9b5b47-41bb05900fe0ec5d","ParentId":"","ConnectionId":"0HM6K6E8THHIM"}
{"@t":"2021-02-18T17:35:24.0938440Z","@m":"Request finished in 3.5198ms 200 application/json; charset=utf-8","@i":"791a596a","ElapsedMilliseconds":3.5198,"StatusCode":200,"ContentType":"application/json; charset=utf-8","HostingRequestFinishedLog":"Request finished in 3.5198ms 200 application/json; charset=utf-8","EventId":{"Id":2},"SourceContext":"Microsoft.AspNetCore.Hosting.Diagnostics","RequestId":"0HM6K6E8THHIM:00000001","RequestPath":"/weatherforecast","SpanId":"|f9b5b47-41bb05900fe0ec5d.","TraceId":"f9b5b47-41bb05900fe0ec5d","ParentId":"","ConnectionId":"0HM6K6E8THHIM"}

```

6. This has "TraceId" same as "CorrelationId", However "CorrelationId" also can be added using serilog enrichers if required.

7. PII data can be excluding on logging uisng Package: Destructurama.ByIgnoring ref: https://github.com/destructurama/by-ignoring

8. Also PII Data can be masked at splunk level, however this is costly operation (Anonymize data) -- To anonymize data with Splunk Cloud, we must configure a Splunk Enterprise instance as a heavy forwarder and anonymize the incoming data with that instance before sending it to Splunk Cloud.

https://docs.splunk.com/Documentation/SplunkCloud/latest/Data/Anonymizedata#:~:text=You%20might%20need%20to%20anonymize,for%20use%20in%20event%20tracking.

## Ref:
* Best Practices ref for dev's: https://benfoster.io/blog/serilog-best-practices/
* Serilog.AspNetCore: This package routes ASP.NET Core log messages through Serilog- https://github.com/serilog/serilog-aspnetcore

## Questions:

 1. Have different HEC token for different environment/Application/Agency -- Ans: Application going to write logs to console & log forwarders will send this logs to splunk.