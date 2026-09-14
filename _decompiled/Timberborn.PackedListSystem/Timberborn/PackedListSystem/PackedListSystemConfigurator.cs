using Bindito.Core;

namespace Timberborn.PackedListSystem;

[Context("Game")]
[Context("MapEditor")]
internal class PackedListSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FloatPackedListSerializer>().AsSingleton();
		Bind<IntPackedListSerializer>().AsSingleton();
		Bind<BoolPackedListSerializer>().AsSingleton();
	}
}
