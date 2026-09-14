using Bindito.Core;

namespace Timberborn.WalkingSystemUI;

[Context("Game")]
internal class WalkingSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WalkerDebugger>().AsSingleton();
	}
}
