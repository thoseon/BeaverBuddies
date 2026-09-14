using Bindito.Core;
using Timberborn.CoreUI;

namespace Timberborn.DropdownSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class DropdownSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DropdownItemsSetter>().AsSingleton();
		Bind<DropdownListDrawer>().AsSingleton();
		Bind<EnumDropdownProviderFactory>().AsSingleton();
		MultiBind<IVisualElementInitializer>().To<DropdownInitializer>().AsSingleton();
	}
}
