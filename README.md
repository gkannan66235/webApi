# Logging guide with Sample ASP.Net application

## Looging Framework:

For .Net structured logging we have top 3 frameworks log4net, nlog and serilog.
- log4net  — battle tested, in system for longtime and 2x growth in usage (nuget download)
- nlog — simple to use, later added support for structured logging, 2x growth in usage
- serilog — simple, built for structured logging and supports out of the box, default json output format, 3x growth in usage
* NuGet package statistics for the [last 6 weeks](https://www.nuget.org/stats/packages)
* Comparison of projects [link](https://www.openhub.net/p/_compare?project_0=Apache+log4net&project_1=serilog&project_2=NLog+-+Advanced+.NET+Logging%20%E2%80%94)
* Sinks available - log4net (lots of custom appenders), nlog-93, serilog-94

What other research says: 
- [Developer Tips](https://stackify.com/nlog-vs-log4net-vs-serilog/)
- [Aspects of ease of use, log speed, and missing logs per total logs](https://medium.com/dev-genius/serilog-vs-nlog-7d0a322a4732)
- [With elmah](https://blog.elmah.io/serilog-vs-log4net/#:~:text=log4net%20doesn't%20support%20the,Serilog%20example%20isn't%20available.)
- [User reactions](https://www.slant.co/topics/61/versus/~nlog_vs_serilog_vs_log4net)

**Conclusion:** Based on user research and developers feedback serilog taken as logging tool.

## Assumptions:
* Logging destination : Splunk Cloud
* Output : Console, WebApp logging system will read console logs and forwards to Splunk.

## For configuration:

1. Provide logging configuration through file (appsettings.json) using [serilog-settings-configuration](https://github.com/serilog/serilog-settings-configuration) for different application/env.
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
.WriteTo.Console(
                                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
```                                    
Log Output:
```
[13:36:13  INF] Request starting HTTP/1.1 GET http://localhost:5000/weatherforecast   {"Protocol": "HTTP/1.1", "Method": "GET", "ContentType": null, "ContentLength": null, "Scheme": "http", "Host": "localhost:5000", "PathBase": "", "Path": "/weatherforecast", "QueryString": "", "EventId": {"Id": 1}, "SourceContext": "Microsoft.AspNetCore.Hosting.Diagnostics", "RequestId": "0HM6J3BI3Q8R6:00000001", "RequestPath": "/weatherforecast", "SpanId": "|bb250333-44973bf0a0de9702.", "TraceId": "bb250333-44973bf0a0de9702", "ParentId": "", "ConnectionId": "0HM6J3BI3Q8R6"}
[13:36:13  INF] Executing endpoint 'webApi.Controllers.WeatherForecastController.Get (webApi)' {"EventId": {"Name": "ExecutingEndpoint"}, "SourceContext": "Microsoft.AspNetCore.Routing.EndpointMiddleware", "RequestId": "0HM6J3BI3Q8R6:00000001", "RequestPath": "/weatherforecast", "SpanId": "|bb250333-44973bf0a0de9702.", "TraceId": "bb250333-44973bf0a0de9702", "ParentId": "", "ConnectionId": "0HM6J3BI3Q8R6"}
[13:36:13  INF] Route matched with {action = "Get", controller = "WeatherForecast"}. Executing controller action with signature System.Collections.Generic.IEnumerable`1[webApi.WeatherForecast] Get() on controller webApi.Controllers.WeatherForecastController (webApi). {"EventId": {"Id": 3, "Name": "ControllerActionExecuting"}, "SourceContext": "Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", "ActionId": "818f6827-49de-414b-bdca-57c2d0b4b43c", "ActionName": "webApi.Controllers.WeatherForecastController.Get (webApi)", "RequestId": "0HM6J3BI3Q8R6:00000001", "RequestPath": "/weatherforecast", "SpanId": "|bb250333-44973bf0a0de9702.", "TraceId": "bb250333-44973bf0a0de9702", "ParentId": "", "ConnectionId": "0HM6J3BI3Q8R6"}
[13:36:13  INF] called claim az500 {"SourceContext": "webApi.Controllers.WeatherForecastController", "ActionId": "818f6827-49de-414b-bdca-57c2d0b4b43c", "ActionName": "webApi.Controllers.WeatherForecastController.Get (webApi)", "RequestId": "0HM6J3BI3Q8R6:00000001", "RequestPath": "/weatherforecast", "SpanId": "|bb250333-44973bf0a0de9702.", "TraceId": "bb250333-44973bf0a0de9702", "ParentId": "", "ConnectionId": "0HM6J3BI3Q8R6"}
[13:36:13  INF] Executing ObjectResult, writing value of type 'webApi.WeatherForecast[]'. {"EventId": {"Id": 1, "Name": "ObjectResultExecuting"}, "SourceContext": "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", "ActionId": "818f6827-49de-414b-bdca-57c2d0b4b43c", "ActionName": "webApi.Controllers.WeatherForecastController.Get (webApi)", "RequestId": "0HM6J3BI3Q8R6:00000001", "RequestPath": "/weatherforecast", "SpanId": "|bb250333-44973bf0a0de9702.", "TraceId": "bb250333-44973bf0a0de9702", "ParentId": "", "ConnectionId": "0HM6J3BI3Q8R6"}
[13:36:13  INF] Executed action webApi.Controllers.WeatherForecastController.Get (webApi) in 2.3038ms {"EventId": {"Id": 2, "Name": "ActionExecuted"}, "SourceContext": "Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", "ActionId": "818f6827-49de-414b-bdca-57c2d0b4b43c", "RequestId": "0HM6J3BI3Q8R6:00000001", "RequestPath": "/weatherforecast", "SpanId": "|bb250333-44973bf0a0de9702.", "TraceId": "bb250333-44973bf0a0de9702", "ParentId": "", "ConnectionId": "0HM6J3BI3Q8R6"}
[13:36:13  INF] Executed endpoint 'webApi.Controllers.WeatherForecastController.Get (webApi)' {"EventId": {"Id": 1, "Name": "ExecutedEndpoint"}, "SourceContext": "Microsoft.AspNetCore.Routing.EndpointMiddleware", "RequestId": "0HM6J3BI3Q8R6:00000001", "RequestPath": "/weatherforecast", "SpanId": "|bb250333-44973bf0a0de9702.", "TraceId": "bb250333-44973bf0a0de9702", "ParentId": "", "ConnectionId": "0HM6J3BI3Q8R6"}
[13:36:13  INF] Request finished in 4.5189ms 200 application/json; charset=utf-8 {"ElapsedMilliseconds": 4.5189, "StatusCode": 200, "ContentType": "application/json; charset=utf-8", "EventId": {"Id": 2}, "SourceContext": "Microsoft.AspNetCore.Hosting.Diagnostics", "RequestId": "0HM6J3BI3Q8R6:00000001", "RequestPath": "/weatherforecast", "SpanId": "|bb250333-44973bf0a0de9702.", "TraceId": "bb250333-44973bf0a0de9702", "ParentId": "", "ConnectionId": "0HM6J3BI3Q8R6"}
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