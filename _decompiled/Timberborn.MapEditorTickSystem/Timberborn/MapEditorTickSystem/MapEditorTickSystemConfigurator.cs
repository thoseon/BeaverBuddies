using Bindito.Core;
using Timberborn.TickSystem;

namespace Timberborn.MapEditorTickSystem;

[Context("MapEditor")]
internal class MapEditorTickSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ITickingMode>().To<MapEditorTickingMode>().AsSingleton();
	}
}
