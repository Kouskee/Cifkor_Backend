using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BreedsView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform _content;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _loadingIndicator;
    [SerializeField] private PopupView _detailsPopup;
    [SerializeField] private ScrollRect _scrollRect;

    private readonly Subject<string> _breedSelectedSubject = new();
    private readonly List<BreedItemView> _breedItems = new();

    public IObservable<string> OnBreedSelected => _breedSelectedSubject.AsObservable();

    private void Awake()
    {
        if (_detailsPopup != null)
            _detailsPopup.gameObject.SetActive(false);
    }

    public void ShowLoadingIndicator(bool show)
    {
        if (_loadingIndicator != null)
            _loadingIndicator.SetActive(show);
    }

    public void DisplayBreeds(List<BreedModel> breeds)
    {
        ClearBreedsList();

        for (int i = 0; i < breeds.Count; i++)
        {
            var breed = breeds[i];
            var breedItem = CreateBreedItem(breed, i + 1);
            _breedItems.Add(breedItem);
        }

        if (_scrollRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
            _scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private BreedItemView CreateBreedItem(BreedModel breed, int index)
    {
        var itemGO = Instantiate(_itemPrefab, _content);
        var breedItem = itemGO.GetComponent<BreedItemView>();

        if (breedItem == null)
        {
            breedItem = itemGO.AddComponent<BreedItemView>();
        }

        breedItem.Setup(breed, index);
        breedItem.OnClicked.Subscribe(breedId => _breedSelectedSubject.OnNext(breedId))
                           .AddTo(this);

        return breedItem;
    }

    public void ShowBreedDetails(BreedModel breed)
    {
        if (_detailsPopup != null)
        {
            _detailsPopup.Show(breed.name, breed.description);
        }
    }

    public void HideBreedDetails()
    {
        if (_detailsPopup != null)
        {
            _detailsPopup.Hide();
        }
    }

    private void ClearBreedsList()
    {
        foreach (var item in _breedItems)
        {
            if (item != null && item.gameObject != null)
            {
                Destroy(item.gameObject);
            }
        }
        _breedItems.Clear();
    }

    private void OnDestroy()
    {
        _breedSelectedSubject?.Dispose();
        ClearBreedsList();
    }
}