using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.MechanicalSystem;
using Timberborn.TickSystem;

namespace Timberborn.GoodConsumingBuildingSystem;

internal class PoweredGoodConsumingBuilding : TickableComponent, IAwakableComponent, IInitializableEntity
{
	private GoodConsumingBuilding _goodConsumingBuilding;

	private MechanicalBuilding _mechanicalBuilding;

	private GoodConsumingToggle _goodConsumingToggle;

	public void Awake()
	{
		_goodConsumingBuilding = GetComponent<GoodConsumingBuilding>();
		_mechanicalBuilding = GetComponent<MechanicalBuilding>();
		_goodConsumingToggle = _goodConsumingBuilding.GetGoodConsumingToggle();
	}

	public void InitializeEntity()
	{
		UpdateState();
	}

	public override void Tick()
	{
		UpdateState();
	}

	private void UpdateState()
	{
		_mechanicalBuilding.SetConsumptionDisabled(!_goodConsumingBuilding.CanUse);
		if (_mechanicalBuilding.ActiveAndPowered)
		{
			_goodConsumingToggle.ResumeConsumption();
		}
		else
		{
			_goodConsumingToggle.PauseConsumption();
		}
	}
}
