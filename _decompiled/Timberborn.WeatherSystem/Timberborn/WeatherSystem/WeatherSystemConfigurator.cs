using Bindito.Core;
using Timberborn.GameCycleSystem;

namespace Timberborn.WeatherSystem;

[Context("Game")]
[Context("MapEditor")]
internal class WeatherSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WeatherService>().AsSingleton();
		Bind<TemperateWeatherDurationService>().AsSingleton();
		Bind<WeatherFastForwarder>().AsSingleton();
		MultiBind<ICycleDuration>().ToExisting<TemperateWeatherDurationService>();
	}
}
