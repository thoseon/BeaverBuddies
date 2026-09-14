using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.WaterSystemRendering;

[Context("Game")]
[Context("MapEditor")]
internal class WaterSystemRenderingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IWaterMesh>().To<WaterMesh>().AsSingleton();
		Bind<IWaterRenderer>().To<WaterRenderer>().AsSingleton();
		Bind<WaterColumnPostprocessor>().AsSingleton();
		Bind<WaterOpacityService>().AsSingleton();
		Bind<WaterBackfacesRenderer>().AsSingleton();
		Bind<WaterRenderingTaskStarter>().AsSingleton();
		Bind<WaterFlowLimitUpdater>().AsSingleton();
		MultiBind<IDevModule>().To<WaterSystemRenderingDevModule>().AsSingleton();
		MultiBind<IDevModule>().To<WaterOpacityOverrider>().AsSingleton();
	}
}
