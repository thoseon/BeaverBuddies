using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.TickSystem;
using Timberborn.WaterSourceSystem;

namespace Timberborn.GameWaterSourceSystem;

internal class UndergroundWaterSourceDrillSounds : TickableComponent, IAwakableComponent, IFinishedStateListener
{
	private BuildingSounds _buildingSounds;

	private UnderlyingWaterSource _underlyingWaterSource;

	public void Awake()
	{
		_buildingSounds = GetComponent<BuildingSounds>();
		_underlyingWaterSource = GetComponent<UnderlyingWaterSource>();
		DisableComponent();
	}

	public override void Tick()
	{
		UpdateSound();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	private void UpdateSound()
	{
		_buildingSounds.ToggleSound(_underlyingWaterSource.WaterSource.CurrentStrength > 0f);
	}
}
