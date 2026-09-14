using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.TimeSystemUI;

[Context("Game")]
internal class TimeSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ClockPanel>().AsSingleton();
		Bind<TimeScaleDebuggingPanel>().AsSingleton();
		Bind<ClockDebuggingPanel>().AsSingleton();
		MultiBind<IDevModule>().To<SpeedControlPanel>().AsSingleton();
		MultiBind<IDevModule>().To<TimeFastForwarderDevModule>().AsSingleton();
		MultiBind<IDevModule>().To<StopwatchDevModule>().AsSingleton();
	}
}
