using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherItemView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _iconText;
    [SerializeField] private TMP_Text temperatureText;
    [SerializeField] private TMP_Text lastUpdatedText;

    [Header("Visual Effects")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Color _dayTimeColor = new(0.5f, 0.8f, 1f, 0.3f);
    [SerializeField] private Color _nightTimeColor = new(0.2f, 0.2f, 0.4f, 0.3f);

    private WeatherModel _currentWeather;

    public void UpdateWeather(WeatherModel weather)
    {
        if (weather == null)
        {
            Debug.LogWarning("Cannot update weather item with null data");
            return;
        }

        _currentWeather = weather;

        if (_iconText != null)
            _iconText.text = weather.icon;

        if (temperatureText != null)
        {
            string displayText = $"Сегодня - {weather.temperature}";
            temperatureText.text = displayText;
        }

        if (lastUpdatedText != null)
            lastUpdatedText.text = $"Обновлено: {weather.timestamp:HH:mm:ss}";

        UpdateVisualAppearance(weather.isDaytime);
    }

    private void UpdateVisualAppearance(bool isDaytime)
    {
        if (_backgroundImage != null)
            _backgroundImage.color = isDaytime ? _dayTimeColor : _nightTimeColor;
    }

    public WeatherModel GetCurrentWeather()
    {
        return _currentWeather;
    }
}
