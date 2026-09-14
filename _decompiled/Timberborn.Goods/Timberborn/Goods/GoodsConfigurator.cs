using Bindito.Core;

namespace Timberborn.Goods;

[Context("Game")]
[Context("MapEditor")]
internal class GoodsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GoodsGroupSpecService>().AsSingleton();
		Bind<GoodIconVisualizer>().AsSingleton();
		Bind<GoodAmountSerializer>().AsSingleton();
		Bind<GoodRegistryValueSerializer>().AsSingleton();
		Bind<SerializedGoodValueSerializer>().AsSingleton();
		Bind<IGoodService>().To<GoodService>().AsSingleton();
	}
}
