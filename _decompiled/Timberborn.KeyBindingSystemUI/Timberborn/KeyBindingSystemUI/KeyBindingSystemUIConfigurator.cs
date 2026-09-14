using Bindito.Core;

namespace Timberborn.KeyBindingSystemUI;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class KeyBindingSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<KeyBindingRowFactory>().AsSingleton();
		Bind<KeyBindingsBox>().AsSingleton();
		Bind<KeyBindingShortcutService>().AsSingleton();
		Bind<KeyBindingShortcutUpdater>().AsSingleton();
		Bind<KeyBindingTooltipFactory>().AsSingleton();
		Bind<KeyRebinder>().AsSingleton();
		Bind<InputBindingDescriber>().AsSingleton();
		Bind<KeyBindingDescriber>().AsSingleton();
		Bind<FixedKeyBindingElementFactory>().AsSingleton();
	}
}
