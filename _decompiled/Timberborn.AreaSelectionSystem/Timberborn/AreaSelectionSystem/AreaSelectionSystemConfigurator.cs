using Bindito.Core;

namespace Timberborn.AreaSelectionSystem;

[Context("Game")]
[Context("MapEditor")]
internal class AreaSelectionSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AreaBoundsDrawingBlocker>().AsTransient();
		Bind<AreaBlockObjectPickerFactory>().AsSingleton();
		Bind<RectangleBoundsDrawerFactory>().AsSingleton();
		Bind<AreaSelector>().AsSingleton();
		Bind<AreaBlockObjectAndTerrainPicker>().AsSingleton();
		Bind<AreaPicker>().AsSingleton();
		Bind<SculptingTerrainPicker>().AsSingleton();
		Bind<AreaSelectionController>().AsSingleton();
	}
}
