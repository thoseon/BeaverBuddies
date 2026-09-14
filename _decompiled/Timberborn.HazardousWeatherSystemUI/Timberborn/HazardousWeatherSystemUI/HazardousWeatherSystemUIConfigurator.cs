using Bindito.Core;

namespace Timberborn.HazardousWeatherSystemUI;

[Context("Game")]
internal class HazardousWeatherSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<HazardousWeatherUIHelper>().AsSingleton();
		Bind<DroughtWeatherUISpecification>().AsSingleton();
		Bind<BadtideWeatherUISpecification>().AsSingleton();
		Bind<HazardousWeatherNotificationPanel>().AsSingleton();
		Bind<HazardousWeatherApproachingTimer>().AsSingleton();
		Bind<HazardousWeatherSoundPlayer>().AsSingleton();
	}
}
