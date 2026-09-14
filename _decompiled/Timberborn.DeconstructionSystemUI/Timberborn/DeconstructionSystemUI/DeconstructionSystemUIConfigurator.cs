using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.DeconstructionSystemUI;

[Context("Game")]
internal class DeconstructionSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuildingDeconstructionTool>().AsSingleton();
		Bind<DeconstructionSoundPlayer>().AsSingleton();
		MultiBind<IDevModule>().To<BuildingDeconstructionToolPreviewDisabler>().AsSingleton();
	}
}
