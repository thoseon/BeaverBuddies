using Bindito.Core;

namespace Timberborn.InputSystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class InputSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BindableButtonFactory>().AsSingleton();
		Bind<BindableToggleFactory>().AsSingleton();
		Bind<KeywordMatchNotifier>().AsSingleton();
	}
}
