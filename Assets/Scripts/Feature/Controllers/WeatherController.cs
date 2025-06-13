using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using Zenject;

public class WeatherController : IDisposable, IInitializable
{
    private readonly IWeatherService _weatherService;
    private readonly INavigationService _navigationService;
    private readonly WeatherView _weatherView;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    private CancellationTokenSource _currentRequestCancellation;
    private bool _isWeatherTabActive = false;

    public WeatherController(
        IWeatherService weatherService,
        INavigationService navigationService,
        WeatherView weatherView)
    {
        _weatherService = weatherService;
        _navigationService = navigationService;
        _weatherView = weatherView;
    }

    public void Initialize()
    {
        _navigationService.CurrentTab
            .Subscribe(OnTabChanged)
            .AddTo(_disposables);

        _weatherService.WeatherUpdates
            .Subscribe(OnWeatherUpdated)
            .AddTo(_disposables);
    }

    private void OnTabChanged(TabType tabType)
    {
        bool wasActive = _isWeatherTabActive;
        _isWeatherTabActive = tabType == TabType.Weather;

        if (_isWeatherTabActive && !wasActive)
        {
            OnWeatherTabActivated();
        }
        else if (!_isWeatherTabActive && wasActive)
        {
            OnWeatherTabDeactivated();
        }
    }

    private void OnWeatherTabActivated()
    {
        _weatherService.StartPeriodicUpdates();
        LoadInitialWeatherData().Forget();
    }

    private void OnWeatherTabDeactivated()
    {
        _weatherService.StopPeriodicUpdates();
        CancelCurrentRequest();
        _weatherView.ShowLoadingIndicator(false);
    }

    private async UniTaskVoid LoadInitialWeatherData()
    {
        try
        {
            if (_weatherService.IsUpdating)
                return;

            CancelCurrentRequest();
            _currentRequestCancellation = new CancellationTokenSource();

            _weatherView.ShowLoadingIndicator(true);

            var weather = await _weatherService.GetWeatherAsync(_currentRequestCancellation.Token);

            _weatherView.ShowLoadingIndicator(false);
            _weatherView.DisplayWeather(weather);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _weatherView.ShowLoadingIndicator(false);
            _weatherView.ShowError($"Failed to load weather: {ex.Message}");
            Debug.LogError($"Failed to load initial weather: {ex.Message}");
        }
        finally
        {
            _currentRequestCancellation?.Dispose();
            _currentRequestCancellation = null;
        }
    }

    private void OnWeatherUpdated(WeatherModel weather)
    {
        if (_isWeatherTabActive)
        {
            _weatherView.DisplayWeather(weather);
        }
    }

    private void CancelCurrentRequest()
    {
        if (_currentRequestCancellation != null && !_currentRequestCancellation.IsCancellationRequested)
        {
            _currentRequestCancellation.Cancel();
            _currentRequestCancellation.Dispose();
            _currentRequestCancellation = null;
        }
    }

    public void Dispose()
    {
        CancelCurrentRequest();
        _weatherService?.StopPeriodicUpdates();
        _disposables?.Dispose();
    }
}