using System;

public interface INavigationService
{
    IObservable<TabType> CurrentTab { get; }
    void SwitchTab(TabType tabType);
}