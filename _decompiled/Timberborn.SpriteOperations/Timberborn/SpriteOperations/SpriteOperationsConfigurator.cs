using Bindito.Core;
using Timberborn.BlueprintSystem;

namespace Timberborn.SpriteOperations;

[Context("Game")]
[Context("MapEditor")]
internal class SpriteOperationsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SpriteResizer>().AsSingleton();
		Bind<SpriteFlipper>().AsSingleton();
		MultiBind<IDeserializer>().To<UISpriteDeserializer>().AsSingleton();
		MultiBind<IDeserializer>().To<FlippedSpriteDeserializer>().AsSingleton();
	}
}
