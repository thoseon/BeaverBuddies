using Bindito.Core;
using Timberborn.GameCycleSystem;

namespace Timberborn.HazardousWeatherSystem;

[Context("Game")]
[Context("MapEditor")]
internal class HazardousWeatherSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<HazardousWeatherService>().AsSingleton();
		Bind<DroughtWeather>().AsSingleton();
		Bind<BadtideWeather>().AsSingleton();
		Bind<HazardousWeatherRandomizer>().AsSingleton();
		Bind<HazardousWeatherHistory>().AsSingleton();
		Bind<HazardousWeatherHistoryDataSerializer>().AsSingleton();
		MultiBind<ICycleDuration>().ToExisting<HazardousWeatherService>();
	}
}
