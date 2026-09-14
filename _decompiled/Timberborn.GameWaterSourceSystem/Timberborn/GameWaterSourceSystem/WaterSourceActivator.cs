using Timberborn.ActivatorSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.WaterSourceSystem;

namespace Timberborn.GameWaterSourceSystem;

public class WaterSourceActivator : BaseComponent, IActivableComponent, IAwakableComponent, IInitializableEntity, IWaterStrengthModifier
{
	private WaterSource _waterSource;

	private bool _isActive = true;

	private bool _forcedActive;

	public void Awake()
	{
		_waterSource = GetComponent<WaterSource>();
	}

	public void InitializeEntity()
	{
		_waterSource.AddWaterStrengthModifier(this);
	}

	public float GetStrengthModifier()
	{
		if (!_isActive && !_forcedActive)
		{
			return 0f;
		}
		return 1f;
	}

	public void Deactivate()
	{
		_isActive = false;
	}

	public void Activate()
	{
		_isActive = true;
	}

	public void ForceActive()
	{
		_forcedActive = true;
	}

	public void DisableForceActive()
	{
		_forcedActive = false;
	}
}
