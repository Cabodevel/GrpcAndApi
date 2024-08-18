using ApiWeb.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ApiWeb.Services;

public class MainProtoService : MainService.MainServiceBase
{
    private static readonly string[] Summaries = new[]
   {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    public override Task<ForecastResult> WeathetForecast(EmptyRequest request, ServerCallContext context)
    {
        var forecasts = Enumerable.Range(1, 5).Select(index => new Forecast
        {
            Date = DateTime.UtcNow.AddDays(index).ToTimestamp(),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });

        return Task.FromResult(new ForecastResult { Forecast = { forecasts } });
    }

    public override async Task WeathetForecastServerStream(EmptyRequest request, IServerStreamWriter<ForecastResult> responseStream, ServerCallContext context)
    {
        var forecasts = Enumerable.Range(1, 100).Select(index => new Forecast
        {
            Date = DateTime.UtcNow.AddDays(index).ToTimestamp(),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });

        foreach (var forecast in forecasts)
        {
            await responseStream.WriteAsync(new ForecastResult { Forecast = { forecast } });
        }
    }

    public override async Task<ForecastResult> WeathetForecastClientStream(IAsyncStreamReader<EmptyRequest> requestStream, ServerCallContext context)
    {
        var index = 0;
        var forecasts = new List<Forecast>();

        while(await requestStream.MoveNext())
        {

            forecasts.Add(new Forecast
            {
                Date = DateTime.UtcNow.AddDays(++index).ToTimestamp(),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });


        }

        return new ForecastResult
        {
            Forecast = { forecasts }
        };
    }

    public override async Task WeathetForecastClientServerStream(IAsyncStreamReader<EmptyRequest> requestStream, IServerStreamWriter<Forecast> responseStream, ServerCallContext context)
    {
        var index = 0;

        while(await requestStream.MoveNext())
        {
            await responseStream.WriteAsync(
                new Forecast
                {
                    Date = DateTime.UtcNow.AddDays(++index).ToTimestamp(),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                });
        }
    }
}
