using System;

[Serializable]
public class WeatherApiResponse
{
    public WeatherProperties properties;
}

[Serializable]
public class WeatherProperties
{
    public WeatherPeriod[] periods;
}

[Serializable]
public class WeatherPeriod
{
    public int number;
    public string name;
    public int temperature;
    public string temperatureUnit;
    public bool isDaytime;
    public string shortForecast;
    public string detailedForecast;
    public string icon;
}