using Bindito.Core;

namespace Timberborn.Brushes;

[Context("Game")]
[Context("MapEditor")]
internal class BrushesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BrushProbabilityMap>().AsSingleton();
		Bind<BrushShapeIterator>().AsSingleton();
	}
}
