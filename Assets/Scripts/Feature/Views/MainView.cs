using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MainView : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Button _breedsTabButton;
    [SerializeField] private Button _weatherTabButton;

    [Header("Tab Panels")]
    [SerializeField] private GameObject _breedsPanel;
    [SerializeField] private GameObject _weatherPanel;

    [Header("Tab Visual States")]
    [SerializeField] private Color _activeTabColor = Color.white;
    [SerializeField] private Color _inactiveTabColor = Color.gray;

    private readonly Subject<TabType> _tabSelectedSubject = new();

    public IObservable<TabType> OnTabSelected => _tabSelectedSubject.AsObservable();

    private void Awake()
    {
        SetupTabButtons();
    }

    private void SetupTabButtons()
    {
        if (_weatherTabButton != null)
        {
            _weatherTabButton.OnClickAsObservable()
                           .Subscribe(_ => OnTabButtonClicked(TabType.Weather))
                           .AddTo(this);
        }

        if (_breedsTabButton != null)
        {
            _breedsTabButton.OnClickAsObservable()
                          .Subscribe(_ => OnTabButtonClicked(TabType.Breed))
                          .AddTo(this);
        }
    }

    private void OnTabButtonClicked(TabType tabType)
    {
        _tabSelectedSubject.OnNext(tabType);
    }

    public void SwitchToTab(TabType tabType)
    {
        if (_weatherPanel != null)
            _weatherPanel.SetActive(tabType == TabType.Weather);

        if (_breedsPanel != null)
            _breedsPanel.SetActive(tabType == TabType.Breed);

        UpdateTabButtonStates(tabType);
    }

    private void UpdateTabButtonStates(TabType activeTab)
    {
        if (_weatherTabButton != null)
        {
            if (_weatherTabButton.TryGetComponent<Image>(out var weatherImage))
                weatherImage.color = activeTab == TabType.Weather ? _activeTabColor : _inactiveTabColor;
        }

        if (_breedsTabButton != null)
        {
            if (_breedsTabButton.TryGetComponent<Image>(out var breedsImage))
                breedsImage.color = activeTab == TabType.Breed ? _activeTabColor : _inactiveTabColor;
        }
    }

    private void OnDestroy()
    {
        _tabSelectedSubject?.Dispose();
    }
}