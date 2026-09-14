using Bindito.Core;
using Timberborn.ModdingUI;

namespace Timberborn.MainMenuModdingUI;

[Context("MainMenu")]
internal class MainMenuModdingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ModManagerBox>().AsSingleton();
		Bind<CreateModBox>().AsSingleton();
		Bind<ModCreator>().AsSingleton();
		Bind<ModUploaderBox>().AsSingleton();
		Bind<IModManagerTooltipRegistrar>().To<ModManagerBoxTooltipRegistrar>().AsSingleton();
		Bind<IModItemFactory>().To<MainMenuModItemFactory>().AsSingleton();
		Bind<ModTemplateDropdownProvider>().AsSingleton();
	}
}
