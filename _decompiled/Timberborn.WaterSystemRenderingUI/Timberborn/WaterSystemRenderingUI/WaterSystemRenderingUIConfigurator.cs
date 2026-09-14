using Bindito.Core;

namespace Timberborn.WaterSystemRenderingUI;

[Context("Game")]
internal class WaterSystemRenderingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WaterRenderingDebuggingPanel>().AsSingleton();
		Bind<WaterRenderingTimeDebuggingPanel>().AsSingleton();
	}
}
