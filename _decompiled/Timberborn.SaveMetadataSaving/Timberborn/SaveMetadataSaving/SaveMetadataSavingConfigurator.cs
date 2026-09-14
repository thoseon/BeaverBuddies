using Bindito.Core;
using Timberborn.SaveSystem;

namespace Timberborn.SaveMetadataSaving;

[Context("Game")]
internal class SaveMetadataSavingConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<ISaveEntryWriter>().To<SaveMetadataSaveEntryWriter>().AsSingleton();
	}
}
