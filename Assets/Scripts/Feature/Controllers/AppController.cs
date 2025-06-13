using System;
using UniRx;
using Zenject;

public class AppController : IDisposable, IInitializable
{
    private readonly INavigationService _navigationService;
    private readonly MainView _mainView;
    private readonly BreedsController _breedsController;
    private readonly WeatherController _weatherController;
    private readonly CompositeDisposable _disposables = new();

    public AppController(
        INavigationService navigationService,
        MainView mainView,
        BreedsController breedsController,
        WeatherController weatherController)
    {
        _navigationService = navigationService;
        _mainView = mainView;
        _breedsController = breedsController;
        _weatherController = weatherController;
    }

    public void Initialize()
    {
        _mainView.OnTabSelected
                 .Subscribe(OnTabSelected)
                 .AddTo(_disposables);

        _navigationService.CurrentTab
                         .Subscribe(OnNavigationChanged)
                         .AddTo(_disposables);
    }

    private void OnTabSelected(TabType tabType) =>
        _navigationService.SwitchTab(tabType);

    private void OnNavigationChanged(TabType tabType) =>
        _mainView.SwitchToTab(tabType);

    public void Dispose()
    {
        _disposables?.Dispose();
        _breedsController?.Dispose();
        _weatherController?.Dispose();
    }
}