using Bindito.Core;

namespace Timberborn.SaveMetadataSystem;

[Context("MainMenu")]
[Context("Game")]
internal class SaveMetadataSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SaveMetadataSerializer>().AsSingleton();
		Bind<ModReferenceSerializer>().AsSingleton();
	}
}
