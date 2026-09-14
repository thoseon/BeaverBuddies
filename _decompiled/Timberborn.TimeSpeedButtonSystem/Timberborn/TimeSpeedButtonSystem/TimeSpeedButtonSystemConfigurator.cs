using Bindito.Core;

namespace Timberborn.TimeSpeedButtonSystem;

[Context("Game")]
[Context("MapEditor")]
internal class TimeSpeedButtonSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TimeSpeedButtonGroup>().AsTransient();
		Bind<TimeSpeedButtonFactory>().AsSingleton();
	}
}
