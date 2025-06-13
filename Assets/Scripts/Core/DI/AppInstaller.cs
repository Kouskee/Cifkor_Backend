using UnityEngine;
using Zenject;

public class AppInstaller : MonoInstaller
{
    [SerializeField] private MainView _mainView;
    [SerializeField] private BreedsView _breedView;
    [SerializeField] private WeatherView _weatherView;
    [Space]
    [SerializeField] private BreedItemView _breedItemPrefab;

    public override void InstallBindings()
    {
        // Services
        Container.Bind<IWeatherService>().To<WeatherService>().AsSingle();
        Container.Bind<IBreedService>().To<BreedService>().AsSingle();
        Container.Bind<INavigationService>().To<NavigationService>().AsSingle();

        // Views
        Container.Bind<MainView>().FromInstance(_mainView).AsSingle();
        Container.Bind<WeatherView>().FromInstance(_weatherView).AsSingle();
        Container.Bind<BreedsView>().FromInstance(_breedView).AsSingle();

        Container.Bind<BreedItemView>().FromComponentInNewPrefab(_breedItemPrefab).AsTransient();

        // Controllers
        Container.BindInterfacesAndSelfTo<AppController>().AsSingle();
        Container.BindInterfacesAndSelfTo<WeatherController>().AsSingle();
        Container.BindInterfacesAndSelfTo<BreedsController>().AsSingle();

        //Factory
        Container.BindFactory<BreedItemView, PlaceholderFactory<BreedItemView>>()
            .FromComponentInNewPrefab(_breedItemPrefab)
            .WithGameObjectName("BreedItem");
    }
}