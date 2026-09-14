using Bindito.Core;

namespace Timberborn.ModdingUI;

[Context("MainMenu")]
internal class ModdingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ModListView>().AsSingleton();
	}
}
