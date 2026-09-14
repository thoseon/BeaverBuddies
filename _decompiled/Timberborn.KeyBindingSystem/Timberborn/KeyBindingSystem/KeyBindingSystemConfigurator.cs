using Bindito.Core;

namespace Timberborn.KeyBindingSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class KeyBindingSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<InputBindingListener>().AsSingleton();
		Bind<InputBindingNameService>().AsSingleton();
		Bind<CustomInputBindingSerializer>().AsSingleton();
		Bind<InputModifiersService>().AsSingleton();
		Bind<InputUpdater>().AsSingleton();
		Bind<KeyBindingDeviceUpdater>().AsSingleton();
		Bind<KeyBindingGroupSpecService>().AsSingleton();
		Bind<KeyBindingRegistry>().AsSingleton();
		Bind<KeyBindingSpecService>().AsSingleton();
		Bind<CtrlKeyOverwriter>().AsSingleton();
		Bind<KeyBindingFactory>().AsSingleton();
	}
}
