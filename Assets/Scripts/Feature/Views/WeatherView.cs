using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private WeatherItemView _weatherItem;
    [SerializeField] private GameObject _loadingIndicator;
    [SerializeField] private GameObject _errorPanel;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private Button _retryButton;

    private void Awake()
    {
        if (_retryButton != null)
            _retryButton.onClick.AddListener(OnRetryClicked);

        ShowError(null);
    }

    public void DisplayWeather(WeatherModel weather)
    {
        if (weather == null)
        {
            Debug.LogWarning("Attempted to display null weather data");
            return;
        }

        ShowError(null);

        if (_weatherItem != null)
        {
            _weatherItem.gameObject.SetActive(true);
            _weatherItem.UpdateWeather(weather);
        }
        else
        {
            Debug.LogWarning("WeatherItemView is not assigned");
        }
    }

    public void ShowLoadingIndicator(bool show)
    {
        if (_loadingIndicator != null)
            _loadingIndicator.SetActive(show);

        if (_weatherItem != null && show)
            _weatherItem.gameObject.SetActive(false);
    }

    public void ShowError(string errorMessage)
    {
        bool hasError = !string.IsNullOrEmpty(errorMessage);

        if (_errorPanel != null)
            _errorPanel.SetActive(hasError);

        if (_errorText != null && hasError)
            _errorText.text = errorMessage;

        if (_weatherItem != null && hasError)
            _weatherItem.gameObject.SetActive(false);
    }

    private void OnRetryClicked()
    {
        ShowError(null);
    }

    private void OnDestroy()
    {
        if (_retryButton != null)
            _retryButton.onClick.RemoveListener(OnRetryClicked);
    }
}