using Bindito.Core;

namespace Timberborn.FactionGoalsSystem;

[Context("Game")]
internal class FactionGoalsSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FactionGoalsUnlocker>().AsSingleton();
	}
}
