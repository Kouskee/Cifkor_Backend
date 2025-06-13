using System;
using System.Collections.Generic;

public class CompositeDisposable : IDisposable
{
    private readonly List<IDisposable> _disposables = new List<IDisposable>();
    private bool _disposed = false;

    public void Add(IDisposable disposable)
    {
        if (disposable == null)
            throw new ArgumentNullException(nameof(disposable));

        if (_disposed)
        {
            disposable.Dispose();
            return;
        }

        _disposables.Add(disposable);
    }

    public void Remove(IDisposable disposable)
    {
        if (disposable == null || _disposed)
            return;

        _disposables.Remove(disposable);
    }

    public void Clear()
    {
        if (_disposed)
            return;

        foreach (var disposable in _disposables)
        {
            try
            {
                disposable?.Dispose();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error disposing: {ex.Message}");
            }
        }

        _disposables.Clear();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        Clear();
        _disposed = true;
    }
}
