using Bindito.Core;
using Timberborn.BlueprintSystem;

namespace Timberborn.LocalizationSerialization;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class LocalizationSerializationConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<IDeserializer>().To<LocalizedTextDeserializer>().AsSingleton();
	}
}
