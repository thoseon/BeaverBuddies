using Bindito.Core;

namespace Timberborn.GridTraversing;

[Context("Game")]
[Context("MapEditor")]
internal class GridTraversingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GridTraversal>().AsSingleton();
	}
}
