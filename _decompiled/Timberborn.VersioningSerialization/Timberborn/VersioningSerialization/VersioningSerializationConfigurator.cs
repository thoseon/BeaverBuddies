using Bindito.Core;
using Timberborn.SaveSystem;

namespace Timberborn.VersioningSerialization;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class VersioningSerializationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<VersionSerializer>().AsSingleton();
		MultiBind<ISaveEntryWriter>().ToExisting<VersionSerializer>();
	}
}
