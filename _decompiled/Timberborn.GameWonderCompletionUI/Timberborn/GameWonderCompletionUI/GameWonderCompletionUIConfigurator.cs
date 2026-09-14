using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.GameWonderCompletionUI;

[Context("Game")]
internal class GameWonderCompletionUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<StatisticItemFactory>().AsSingleton();
		Bind<WonderCompletionPanel>().AsSingleton();
		MultiBind<IDevModule>().To<WonderCompletionDevModule>().AsSingleton();
	}
}
