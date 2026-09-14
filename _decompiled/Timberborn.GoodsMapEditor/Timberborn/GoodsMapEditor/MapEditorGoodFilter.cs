using Timberborn.Goods;

namespace Timberborn.GoodsMapEditor;

internal class MapEditorGoodFilter : IGoodFilter
{
	public bool IsUsable(GoodSpec goodSpec)
	{
		return true;
	}
}
