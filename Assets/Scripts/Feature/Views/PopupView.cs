using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _background;
    [Space]
    [SerializeField] private RectTransform _contentRectTransform;
    [SerializeField] private VerticalLayoutGroup _contentLayoutGroup;

    [Header("Adaptive Settings")]
    [SerializeField] private float _topMargin = 100f;
    [SerializeField] private float _bottomMargin = 100f;
    [SerializeField] private float _maxFontSize = 16;

    private float _originalFontSize;

    private RectTransform _descriptionRectTransform;
    private ContentSizeFitter _descriptionSizeFitter;
    private RectTransform _closeButtonRectTransform;

    private void Awake()
    {
        _descriptionRectTransform = _descriptionText.GetComponent<RectTransform>();
        _descriptionSizeFitter = _descriptionText.GetComponent<ContentSizeFitter>();
        _closeButtonRectTransform = _closeButton.GetComponent<RectTransform>();

        if (_descriptionText != null)
        {
            _originalFontSize = _descriptionText.fontSize;
            _maxFontSize = Mathf.Max(_maxFontSize, _originalFontSize);
        }

        if (_closeButton != null)
        {
            _closeButton.OnClickAsObservable()
                      .Subscribe(_ => Hide())
                      .AddTo(this);
        }

        if (_background != null)
        {
            _background.OnClickAsObservable()
                           .Subscribe(_ => Hide())
                           .AddTo(this);
        }
    }

    public void Show(string title, string description)
    {
        if (_titleText != null)
            _titleText.text = title;

        if (_descriptionText != null)
        {
            _descriptionText.text = description;
            _descriptionText.fontSize = _maxFontSize;
        }

        gameObject.SetActive(true);

        AdaptHeight();
    }

    private void AdaptHeight()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_descriptionRectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRectTransform);

        var titleHeight = _titleText.preferredHeight;
        var descriptionPreferredHeight = _descriptionText.preferredHeight;
        var buttonHeight = _closeButtonRectTransform.rect.height;

        var paddingTop = _contentLayoutGroup.padding.top;
        var paddingBottom = _contentLayoutGroup.padding.bottom;
        var spacing = _contentLayoutGroup.spacing;
        var childCount = 3;

        var totalPreferredHeight = titleHeight + descriptionPreferredHeight + buttonHeight + paddingTop + paddingBottom + (spacing * (childCount - 1));
        var maxPopupHeight = Screen.height - _topMargin - _bottomMargin;

        if (totalPreferredHeight <= maxPopupHeight)
        {
            _descriptionSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            _descriptionText.enableAutoSizing = false;
        }
        else
        {
            _descriptionSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            var maxDescriptionHeight = maxPopupHeight - titleHeight - buttonHeight - paddingTop - paddingBottom - (spacing * (childCount - 1));
            _descriptionText.rectTransform.sizeDelta = new Vector2(_descriptionText.rectTransform.sizeDelta.x, maxDescriptionHeight);
            _descriptionText.enableAutoSizing = true;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        if (_descriptionText != null)
        {
            _descriptionText.fontSize = _originalFontSize;
        }
    }
}