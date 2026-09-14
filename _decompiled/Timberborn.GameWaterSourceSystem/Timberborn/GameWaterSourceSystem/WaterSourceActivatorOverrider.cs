using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.WaterSourceSystem;

namespace Timberborn.GameWaterSourceSystem;

internal class WaterSourceActivatorOverrider : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private WaterSourceRegulator _waterSourceRegulator;

	private UnderlyingWaterSource _underlyingWaterSource;

	private WaterSourceActivator _waterSourceActivator;

	public void Awake()
	{
		_waterSourceRegulator = GetComponent<WaterSourceRegulator>();
		_underlyingWaterSource = GetComponent<UnderlyingWaterSource>();
	}

	public void OnEnterFinishedState()
	{
		WaterSource waterSource = _underlyingWaterSource.WaterSource;
		if (waterSource != null)
		{
			_waterSourceActivator = waterSource.GetComponent<WaterSourceActivator>();
			_waterSourceRegulator.OpenStateChanged += OnOpenStateChanged;
			UpdateActivatorForcedState(_waterSourceRegulator.IsOpen);
		}
	}

	public void OnExitFinishedState()
	{
		if ((bool)_underlyingWaterSource.WaterSource)
		{
			_waterSourceRegulator.OpenStateChanged -= OnOpenStateChanged;
			UpdateActivatorForcedState(isOpen: false);
		}
	}

	private void OnOpenStateChanged(object sender, bool isOpen)
	{
		UpdateActivatorForcedState(isOpen);
	}

	private void UpdateActivatorForcedState(bool isOpen)
	{
		if (isOpen)
		{
			_waterSourceActivator.ForceActive();
		}
		else
		{
			_waterSourceActivator.DisableForceActive();
		}
	}
}
