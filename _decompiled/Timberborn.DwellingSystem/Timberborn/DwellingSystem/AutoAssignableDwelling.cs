using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Buildings;
using UnityEngine;

namespace Timberborn.DwellingSystem;

internal class AutoAssignableDwelling : BaseComponent, IAwakableComponent, IFinishedPausable, IFinishedStateListener
{
	private readonly StaleAssignableDwellingService _staleAssignableDwellingService;

	private Dwelling _dwelling;

	private BlockableObject _blockableObject;

	public bool HasAssignableSlot
	{
		get
		{
			if (Active)
			{
				return _dwelling.HasFreeSlots;
			}
			return false;
		}
	}

	public bool ShouldAssignAdult
	{
		get
		{
			if (!_dwelling.IsEmpty)
			{
				return !_dwelling.UnderpopulatedByChildren;
			}
			return true;
		}
	}

	private bool Active
	{
		get
		{
			if (base.Enabled)
			{
				return _blockableObject.IsUnblocked;
			}
			return false;
		}
	}

	public AutoAssignableDwelling(StaleAssignableDwellingService staleAssignableDwellingService)
	{
		_staleAssignableDwellingService = staleAssignableDwellingService;
	}

	public void Awake()
	{
		_dwelling = GetComponent<Dwelling>();
		_blockableObject = GetComponent<BlockableObject>();
		_blockableObject.ObjectBlocked += delegate
		{
			_dwelling.UnassignAllDwellers();
		};
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		_staleAssignableDwellingService.RegisterDwelling(this);
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_staleAssignableDwellingService.UnregisterDwelling(this);
	}

	public bool CanAssignDweller(Dweller dweller)
	{
		if (Active && dweller.Home != _dwelling && _dwelling.HasFreeSlots)
		{
			return CanMoveIn(dweller);
		}
		return false;
	}

	public void AssignDweller(Dweller dweller)
	{
		if (!CanAssignDweller(dweller))
		{
			Debug.LogWarning($"Can't assign {dweller} to {base.Name}");
		}
		else
		{
			_dwelling.AssignDweller(dweller);
		}
	}

	private bool CanMoveIn(Dweller dweller)
	{
		if (dweller.HasHome)
		{
			if (!dweller.GetComponent<Child>())
			{
				return CanAdultMoveIn(dweller);
			}
			return CanChildMoveIn(dweller);
		}
		return true;
	}

	private bool CanChildMoveIn(Dweller dweller)
	{
		if (dweller.Home.NumberOfAdultDwellers != 0 || _dwelling.NumberOfAdultDwellers <= 0)
		{
			return _dwelling.UnderpopulatedByChildren;
		}
		return true;
	}

	private bool CanAdultMoveIn(Dweller dweller)
	{
		if (_dwelling.FreeAdultSlots <= dweller.Home.FreeAdultSlots + 1)
		{
			if (dweller.Home.NumberOfDwellers == 1)
			{
				return _dwelling.NumberOfDwellers == 1;
			}
			return false;
		}
		return true;
	}
}
