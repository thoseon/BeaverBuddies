using Bindito.Core;

namespace Timberborn.YielderFinding;

[Context("Game")]
[Context("MapEditor")]
internal class YielderFindingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<YieldStatus>().AsTransient();
		Bind<YielderFinder>().AsSingleton();
		Bind<ClosestYielderFinder>().AsSingleton();
	}
}
