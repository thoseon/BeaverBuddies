using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.WaterSourceSystem;

public class WaterSourceDisabler : BaseComponent, IAwakableComponent, IWaterStrengthModifier, IFinishedStateListener
{
	private UnderlyingWaterSource _underlyingWaterSource;

	public void Awake()
	{
		_underlyingWaterSource = GetComponent<UnderlyingWaterSource>();
	}

	public float GetStrengthModifier()
	{
		return 0f;
	}

	public void OnEnterFinishedState()
	{
		_underlyingWaterSource.AddWaterStrengthModifier(this);
	}

	public void OnExitFinishedState()
	{
		_underlyingWaterSource.RemoveWaterStrengthModifier(this);
	}
}
