using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BreedItemView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _indexText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Button _button;

    private readonly Subject<string> _clickedSubject = new();
    private string _breedId;

    public IObservable<string> OnClicked => _clickedSubject.AsObservable();

    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if (_button != null)
        {
            _button.OnClickAsObservable()
                  .Subscribe(_ => _clickedSubject.OnNext(_breedId))
                  .AddTo(this);
        }
    }

    public void Setup(BreedModel breed, int index)
    {
        _breedId = breed.id;

        if (_indexText != null)
            _indexText.text = index.ToString();

        if (_nameText != null)
            _nameText.text = breed.name;
    }

    private void OnDestroy()
    {
        _clickedSubject?.Dispose();
    }
}