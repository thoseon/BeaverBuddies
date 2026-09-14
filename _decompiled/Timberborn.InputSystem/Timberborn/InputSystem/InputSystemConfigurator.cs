using Bindito.Core;

namespace Timberborn.InputSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class InputSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<CursorService>().AsSingleton();
		Bind<InputService>().AsSingleton();
		Bind<InputSettings>().AsSingleton();
		Bind<IInputStateResetter>().To<InputStateResetter>().AsSingleton();
		Bind<KeyboardListener>().AsSingleton();
		Bind<KeywordService>().AsSingleton();
		Bind<MouseController>().AsSingleton();
		Bind<InputBlocker>().AsSingleton();
	}
}
