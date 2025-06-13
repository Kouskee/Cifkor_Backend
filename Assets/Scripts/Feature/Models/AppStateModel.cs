using System.Collections.Generic;

public class AppStateModel
{
    public TabType currentTab;
    public WeatherModel currentWeather;
    public List<BreedModel> breeds;
    public BreedModel selectedBreed;
    public bool isLoading;
}
