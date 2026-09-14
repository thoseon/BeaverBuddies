namespace Timberborn.Goods;

public interface IGoodFilter
{
	bool IsUsable(GoodSpec goodSpec);
}
