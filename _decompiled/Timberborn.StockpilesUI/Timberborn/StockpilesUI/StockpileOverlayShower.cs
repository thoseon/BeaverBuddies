using Timberborn.InputSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.StockpilesUI;

internal class StockpileOverlayShower : ILoadableSingleton, IInputProcessor
{
	private static readonly string ShowStockpileOverlayKey = "ShowStockpileOverlay";

	private readonly StockpileOverlay _stockpileOverlay;

	private readonly InputService _inputService;

	private StockpileOverlayToggle _stockpileOverlayToggle;

	private bool _isShown;

	public StockpileOverlayShower(StockpileOverlay stockpileOverlay, InputService inputService)
	{
		_stockpileOverlay = stockpileOverlay;
		_inputService = inputService;
	}

	public void Load()
	{
		_stockpileOverlayToggle = _stockpileOverlay.GetStockpileOverlayToggle();
		_inputService.AddInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyHeld(ShowStockpileOverlayKey))
		{
			if (!_isShown)
			{
				Enable();
			}
		}
		else if (_isShown)
		{
			Disable();
		}
		return false;
	}

	private void Enable()
	{
		_isShown = true;
		_stockpileOverlayToggle.EnableOverlay();
	}

	private void Disable()
	{
		_isShown = false;
		_stockpileOverlayToggle.DisableOverlay();
	}
}
