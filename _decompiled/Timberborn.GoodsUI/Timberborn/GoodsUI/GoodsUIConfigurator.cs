using Bindito.Core;

namespace Timberborn.GoodsUI;

[Context("Game")]
[Context("MapEditor")]
internal class GoodsUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GoodDescriber>().AsSingleton();
		Bind<GoodItemFactory>().AsSingleton();
	}
}
