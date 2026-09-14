using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.WeatherSystemUI;

[Context("Game")]
internal class WeatherSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DatePanel>().AsSingleton();
		Bind<WeatherPanel>().AsSingleton();
		Bind<WeatherDebuggingPanel>().AsSingleton();
		MultiBind<IDevModule>().To<WeatherFastForwarderDevModule>().AsSingleton();
	}
}
