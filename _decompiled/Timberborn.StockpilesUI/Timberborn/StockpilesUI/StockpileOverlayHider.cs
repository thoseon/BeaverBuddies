using Timberborn.CoreUI;
using Timberborn.SingletonSystem;

namespace Timberborn.StockpilesUI;

internal class StockpileOverlayHider : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly StockpileOverlay _stockpileOverlay;

	private StockpileOverlayToggle _stockpileOverlayToggle;

	public StockpileOverlayHider(EventBus eventBus, StockpileOverlay stockpileOverlay)
	{
		_eventBus = eventBus;
		_stockpileOverlay = stockpileOverlay;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_stockpileOverlayToggle = _stockpileOverlay.GetStockpileOverlayToggle();
	}

	[OnEvent]
	public void OnUIVisibilityChanged(UIVisibilityChangedEvent uiVisibilityChangedEvent)
	{
		if (uiVisibilityChangedEvent.UIVisible)
		{
			_stockpileOverlayToggle.ShowOverlay();
		}
		else
		{
			_stockpileOverlayToggle.HideOverlay();
		}
	}
}
