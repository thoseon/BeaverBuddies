using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.WaterSourceSystem;

namespace Timberborn.GameWaterSourceSystem;

public class UndergroundWaterSource : BaseComponent, IAwakableComponent, IInitializableEntity, IWaterStrengthModifier
{
	private WaterSource _waterSource;

	private bool _isOccupied;

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
		return _isOccupied ? 1 : 0;
	}

	public void SetOccupied()
	{
		_isOccupied = true;
	}

	public void SetUnoccupied()
	{
		_isOccupied = false;
	}
}
