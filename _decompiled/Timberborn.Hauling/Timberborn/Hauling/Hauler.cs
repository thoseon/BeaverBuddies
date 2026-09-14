using Timberborn.BaseComponentSystem;
using Timberborn.Carrying;
using Timberborn.EntitySystem;
using Timberborn.WorkSystem;

namespace Timberborn.Hauling;

public class Hauler : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private BackpackCarrier _backpackCarrier;

	private Worker _worker;

	private bool HasBackpack => _worker.Workplace.GetComponent<WorkplaceWithBackpacks>();

	public void Awake()
	{
		_backpackCarrier = GetComponent<BackpackCarrier>();
		_worker = GetComponent<Worker>();
		_worker.GotEmployed += delegate
		{
			CarryInBackpackAsHauler();
		};
		_worker.GotUnemployed += delegate
		{
			CarryInHands();
		};
	}

	public void InitializeEntity()
	{
		if (_worker.Employed && HasBackpack)
		{
			_backpackCarrier.EnableBackpack();
		}
		else
		{
			_backpackCarrier.DisableBackpack();
		}
	}

	private void CarryInBackpackAsHauler()
	{
		if (HasBackpack)
		{
			_backpackCarrier.EnableBackpack();
		}
	}

	private void CarryInHands()
	{
		_backpackCarrier.DisableBackpack();
	}
}
