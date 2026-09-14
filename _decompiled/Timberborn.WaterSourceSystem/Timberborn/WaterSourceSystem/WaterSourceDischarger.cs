using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.WaterSourceSystem;

internal class WaterSourceDischarger : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private UnderlyingWaterSource _underlyingWaterSource;

	public void Awake()
	{
		_underlyingWaterSource = GetComponent<UnderlyingWaterSource>();
	}

	public void OnEnterFinishedState()
	{
		_underlyingWaterSource.DisableDroughtInfluence();
	}

	public void OnExitFinishedState()
	{
		_underlyingWaterSource.EnableDroughtInfluence();
	}
}
