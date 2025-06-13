using System;
using UnityEngine;

public class CompositeDisposableComponent : MonoBehaviour
{
    private CompositeDisposable _compositeDisposable = new();

    public void AddDisposable(IDisposable disposable)
    {
        _compositeDisposable.Add(disposable);
    }

    private void OnDestroy()
    {
        _compositeDisposable?.Dispose();
    }
}