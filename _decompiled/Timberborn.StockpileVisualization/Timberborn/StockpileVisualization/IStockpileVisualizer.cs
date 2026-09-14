using Timberborn.Goods;

namespace Timberborn.StockpileVisualization;

internal interface IStockpileVisualizer
{
	bool CanVisualize(string stockpileVisualization);

	void Initialize(GoodSpec goodSpec, int capacity);

	void UpdateAmount(int amountInStock);

	void Clear();
}
