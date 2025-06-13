using System;
using UniRx;

public class NavigationService : INavigationService
{
    private readonly ReactiveProperty<TabType> _currentTab = new(TabType.Breed);

    public IObservable<TabType> CurrentTab => _currentTab.AsObservable();

    public TabType GetCurrentTab() => _currentTab.Value;

    public void SwitchTab(TabType tabType)
    {
        if (_currentTab.Value != tabType)
            _currentTab.Value = tabType;
    }
}