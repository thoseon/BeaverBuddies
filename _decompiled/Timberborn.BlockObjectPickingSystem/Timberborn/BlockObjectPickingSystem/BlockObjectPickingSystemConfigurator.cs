using Bindito.Core;

namespace Timberborn.BlockObjectPickingSystem;

[Context("Game")]
[Context("MapEditor")]
internal class BlockObjectPickingSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockObjectModelBlockadeIgnorer>().AsTransient();
		Bind<BlockObjectRaycaster>().AsSingleton();
		Bind<BlockObjectPicker>().AsSingleton();
		Bind<BlockObjectPreviewPicker>().AsSingleton();
		Bind<StackedBlockObjectPicker>().AsSingleton();
	}
}
