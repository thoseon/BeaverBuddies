using Bindito.Core;

namespace Timberborn.BuilderPrioritySystem;

[Context("Game")]
[Context("MapEditor")]
internal class BuilderPrioritySystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuilderPrioritizable>().AsTransient();
	}
}
