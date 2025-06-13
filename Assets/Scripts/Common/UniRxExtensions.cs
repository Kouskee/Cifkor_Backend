using System;
using UnityEngine;

public static class UniRxExtensions
{
    public static IDisposable AddTo(this IDisposable disposable, CompositeDisposable compositeDisposable)
    {
        compositeDisposable.Add(disposable);
        return disposable;
    }

    public static IDisposable AddTo(this IDisposable disposable, MonoBehaviour monoBehaviour)
    {
        if (monoBehaviour == null)
            return disposable;

        // Create or get existing CompositeDisposable component
        var compositeDisposableComponent = monoBehaviour.GetComponent<CompositeDisposableComponent>();
        if (compositeDisposableComponent == null)
        {
            compositeDisposableComponent = monoBehaviour.gameObject.AddComponent<CompositeDisposableComponent>();
        }

        compositeDisposableComponent.AddDisposable(disposable);
        return disposable;
    }
}