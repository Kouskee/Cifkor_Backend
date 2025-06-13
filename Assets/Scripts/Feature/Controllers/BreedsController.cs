using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using Zenject;

public class BreedsController : IDisposable, IInitializable
{
    private readonly IBreedService _dogApiService;
    private readonly INavigationService _navigationService;
    private readonly BreedsView _breedsView;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    private CancellationTokenSource _currentRequestCancellation;
    private bool _breedsLoaded = false;

    public BreedsController(
        IBreedService dogApiService,
        INavigationService navigationService,
        BreedsView breedsView)
    {
        _dogApiService = dogApiService;
        _navigationService = navigationService;
        _breedsView = breedsView;
    }

    public void Initialize()
    {
        _navigationService.CurrentTab
            .Subscribe(OnTabChanged)
            .AddTo(_disposables);

        _breedsView.OnBreedSelected
            .Subscribe(OnBreedSelected)
            .AddTo(_disposables);
    }

    private void OnTabChanged(TabType tabType)
    {
        if (tabType == TabType.Breed)
        {
            OnBreedsTabActivated();
        }
        else
        {
            OnBreedsTabDeactivated();
        }
    }

    private async void OnBreedsTabActivated()
    {
        if (!_breedsLoaded)
        {
            await LoadBreeds();
        }
    }

    private void OnBreedsTabDeactivated()
    {
        CancelCurrentRequest();
        _breedsView.ShowLoadingIndicator(false);
        _breedsView.HideBreedDetails();
    }

    private async UniTask LoadBreeds()
    {
        try
        {
            CancelCurrentRequest();
            _currentRequestCancellation = new CancellationTokenSource();

            _breedsView.ShowLoadingIndicator(true);

            var breeds = await _dogApiService.GetBreedsAsync(_currentRequestCancellation.Token);

            _breedsView.ShowLoadingIndicator(false);
            _breedsView.DisplayBreeds(breeds);
            _breedsLoaded = true;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _breedsView.ShowLoadingIndicator(false);
            Debug.LogError($"Failed to load breeds: {ex.Message}");
        }
        finally
        {
            _currentRequestCancellation?.Dispose();
            _currentRequestCancellation = null;
        }
    }

    private async void OnBreedSelected(string breedId)
    {
        try
        {
            CancelCurrentRequest();
            _currentRequestCancellation = new CancellationTokenSource();

            _breedsView.HideBreedDetails();
            _breedsView.ShowLoadingIndicator(true);

            var breedDetails = await _dogApiService.GetBreedDetailsAsync(breedId, _currentRequestCancellation.Token);

            _breedsView.ShowLoadingIndicator(false);
            _breedsView.ShowBreedDetails(breedDetails);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _breedsView.ShowLoadingIndicator(false);
            Debug.LogError($"Failed to load breed details for {breedId}: {ex.Message}");
        }
        finally
        {
            _currentRequestCancellation?.Dispose();
            _currentRequestCancellation = null;
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
        _disposables?.Dispose();
    }
}