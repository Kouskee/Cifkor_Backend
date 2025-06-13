using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherService : IWeatherService, IDisposable
{
    private const string WEATHER_API_URL = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
    private const int UPDATE_INTERVAL_SECONDS = 5;

    private readonly Subject<WeatherModel> _weatherUpdates = new Subject<WeatherModel>();
    private CancellationTokenSource _periodicUpdateCancellation;
    private bool _isUpdating = false;

    public IObservable<WeatherModel> WeatherUpdates => _weatherUpdates.AsObservable();
    public bool IsUpdating => _isUpdating;

    public async UniTask<WeatherModel> GetWeatherAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using (var request = UnityWebRequest.Get(WEATHER_API_URL))
            {
                request.SetRequestHeader("Accept", "application/json");
                request.SetRequestHeader("User-Agent", "Unity Weather App");

                await request.SendWebRequest().WithCancellation(cancellationToken);

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"Weather request failed: {request.error}");
                }

                var responseText = request.downloadHandler.text;
                var response = JsonUtility.FromJson<WeatherApiResponse>(responseText);

                if (response?.properties?.periods == null || response.properties.periods.Length == 0)
                {
                    throw new Exception("Invalid weather response format");
                }

                var currentPeriod = response.properties.periods[0];

                return new WeatherModel(
                    temperature: $"{currentPeriod.temperature}°{currentPeriod.temperatureUnit}",
                    description: currentPeriod.shortForecast,
                    icon: GetWeatherIcon(currentPeriod.shortForecast, currentPeriod.isDaytime),
                    name: currentPeriod.name,
                    isDaytime: currentPeriod.isDaytime
                );
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching weather: {ex.Message}");
            throw;
        }
    }

    public void StartPeriodicUpdates()
    {
        if (_isUpdating)
            return;

        _isUpdating = true;
        _periodicUpdateCancellation = new CancellationTokenSource();

        PeriodicUpdateLoop(_periodicUpdateCancellation.Token).Forget();
    }

    public void StopPeriodicUpdates()
    {
        if (!_isUpdating)
            return;

        _isUpdating = false;
        _periodicUpdateCancellation?.Cancel();
        _periodicUpdateCancellation?.Dispose();
        _periodicUpdateCancellation = null;
    }

    private async UniTaskVoid PeriodicUpdateLoop(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var weather = await GetWeatherAsync(cancellationToken);
                    _weatherUpdates.OnNext(weather);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error in periodic weather update: {ex.Message}");
                }

                await UniTask.Delay(TimeSpan.FromSeconds(UPDATE_INTERVAL_SECONDS).Milliseconds, false, PlayerLoopTiming.Update, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private string GetWeatherIcon(string forecast, bool isDaytime)
    {
        var lowerForecast = forecast.ToLower();

        if (lowerForecast.Contains("sunny") || lowerForecast.Contains("clear"))
            return isDaytime ? "солнце" : "луна";
        else if (lowerForecast.Contains("cloudy") || lowerForecast.Contains("overcast"))
            return "пасмурно";
        else if (lowerForecast.Contains("rain") || lowerForecast.Contains("shower"))
            return "дождь";
        else if (lowerForecast.Contains("storm") || lowerForecast.Contains("thunder"))
            return "шторм";
        else if (lowerForecast.Contains("snow"))
            return "снег";
        else if (lowerForecast.Contains("fog") || lowerForecast.Contains("mist"))
            return "туман";
        else if (lowerForecast.Contains("partly") || lowerForecast.Contains("scattered"))
            return isDaytime ? "облачно с прояснением" : "облачная ночь";
        else
            return isDaytime ? "облачно с прояснением" : "луна";
    }

    public void Dispose()
    {
        StopPeriodicUpdates();
        _weatherUpdates?.Dispose();
    }
}