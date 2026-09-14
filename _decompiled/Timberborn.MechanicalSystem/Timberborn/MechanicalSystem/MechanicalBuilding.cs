using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;

namespace Timberborn.MechanicalSystem;

public class MechanicalBuilding : BaseComponent, IAwakableComponent, IFinishedStateListener, IFinishedPausable, IBuildingEfficiencyProvider
{
	private MechanicalNode _mechanicalNode;

	public bool ActiveAndPowered => _mechanicalNode.ActiveAndPowered;

	public bool CanUse => ActiveAndPowered;

	public float Efficiency => _mechanicalNode.PowerEfficiency;

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void SetConsumptionDisabled(bool value)
	{
		if (_mechanicalNode.IsConsumer)
		{
			_mechanicalNode.SetInputMultiplier((!value) ? 1 : 0);
		}
	}
}
