using System;

[Serializable]
public class WeatherModel
{
    public string temperature;
    public string description;
    public string icon;
    public DateTime timestamp;
    public string name;
    public bool isDaytime;

    public WeatherModel(string temperature, string description, string icon = "", string name = "", bool isDaytime = true)
    {
        this.temperature = temperature;
        this.description = description;
        this.icon = icon;
        this.name = name;
        this.isDaytime = isDaytime;
        this.timestamp = DateTime.Now;
    }
}