using Bindito.Core;
using Timberborn.Goods;

namespace Timberborn.GoodsMapEditor;

[Context("MapEditor")]
internal class GoodsMapEditorConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IGoodFilter>().To<MapEditorGoodFilter>().AsSingleton();
	}
}
