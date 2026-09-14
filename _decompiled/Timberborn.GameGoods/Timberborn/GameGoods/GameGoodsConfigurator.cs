using Bindito.Core;
using Timberborn.Goods;

namespace Timberborn.GameGoods;

[Context("Game")]
internal class GameGoodsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IGoodFilter>().To<GameGoodFilter>().AsSingleton();
	}
}
