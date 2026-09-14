using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Navigation;
using Timberborn.TickSystem;

namespace Timberborn.DwellingSystem;

internal class UnreachableHomeUnassigner : TickableComponent, IAwakableComponent, INavMeshListener, IInitializableEntity, IDeletableEntity
{
	private readonly INavMeshListenerEntityRegistry _navMeshListenerEntityRegistry;

	private Dweller _dweller;

	private Citizen _citizen;

	private bool _checkHomeReachability;

	public UnreachableHomeUnassigner(INavMeshListenerEntityRegistry navMeshListenerEntityRegistry)
	{
		_navMeshListenerEntityRegistry = navMeshListenerEntityRegistry;
	}

	public void Awake()
	{
		_dweller = GetComponent<Dweller>();
		_citizen = GetComponent<Citizen>();
	}

	public override void Tick()
	{
		if (_checkHomeReachability)
		{
			UnassignFromHomeIfNotInDistrict();
			_checkHomeReachability = false;
		}
	}

	public void InitializeEntity()
	{
		_citizen.ChangedAssignedDistrict += delegate
		{
			UnassignFromHomeIfNotInDistrict();
		};
		_navMeshListenerEntityRegistry.RegisterNavMeshListener(this);
	}

	public void DeleteEntity()
	{
		_navMeshListenerEntityRegistry.UnregisterNavMeshListener(this);
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		ScheduleDwellingReachabilityCheck();
	}

	private void ScheduleDwellingReachabilityCheck()
	{
		_checkHomeReachability = true;
	}

	private void UnassignFromHomeIfNotInDistrict()
	{
		if (_dweller.HasHome)
		{
			DistrictBuilding component = _dweller.Home.GetComponent<DistrictBuilding>();
			if (!_citizen.HasAssignedDistrict || component.District != _citizen.AssignedDistrict)
			{
				_dweller.UnassignFromHome();
			}
		}
	}
}
