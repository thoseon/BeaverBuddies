using Timberborn.BaseComponentSystem;
using Timberborn.InventorySystem;

namespace Timberborn.StockpileVisualization;

public class StockpileVisualizationUpdater : BaseComponent, IAwakableComponent
{
	private StockpileBannerSetter _stockpileBannerSetter;

	private StockpileVisualizers _stockpileVisualizers;

	private SingleGoodAllower _singleGoodAllower;

	public void Awake()
	{
		_stockpileBannerSetter = GetComponent<StockpileBannerSetter>();
		_stockpileVisualizers = GetComponent<StockpileVisualizers>();
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
	}

	public void UpdateVisualization()
	{
		_stockpileVisualizers?.SetCurrentVisualizer(_singleGoodAllower.AllowedGood);
		_stockpileBannerSetter.UpdateProperties();
	}
}
