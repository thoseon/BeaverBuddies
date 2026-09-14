using Bindito.Core;

namespace Timberborn.SaveSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class SaveSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SaveReader>().AsSingleton();
		Bind<SaveWriter>().AsSingleton();
	}
}
