using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public interface IWeatherService
{
    UniTask<WeatherModel> GetWeatherAsync(CancellationToken cancellationToken = default);
    IObservable<WeatherModel> WeatherUpdates { get; }
    void StartPeriodicUpdates();
    void StopPeriodicUpdates();
    bool IsUpdating { get; }
}