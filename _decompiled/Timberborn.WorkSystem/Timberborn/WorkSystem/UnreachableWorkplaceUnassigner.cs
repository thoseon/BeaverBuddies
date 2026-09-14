using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Navigation;
using Timberborn.TickSystem;

namespace Timberborn.WorkSystem;

public class UnreachableWorkplaceUnassigner : TickableComponent, IAwakableComponent, INavMeshListener, IInitializableEntity, IDeletableEntity
{
	private readonly INavMeshListenerEntityRegistry _navMeshListenerEntityRegistry;

	private Worker _worker;

	private Citizen _citizen;

	private bool _checkWorkplaceReachability;

	public UnreachableWorkplaceUnassigner(INavMeshListenerEntityRegistry navMeshListenerEntityRegistry)
	{
		_navMeshListenerEntityRegistry = navMeshListenerEntityRegistry;
	}

	public void Awake()
	{
		_worker = GetComponent<Worker>();
		_citizen = GetComponent<Citizen>();
	}

	public override void Tick()
	{
		if (_checkWorkplaceReachability)
		{
			UnemployIfWorkplaceIsNotInDistrict();
			_checkWorkplaceReachability = false;
		}
	}

	public void InitializeEntity()
	{
		_citizen.ChangedAssignedDistrict += delegate
		{
			UnemployIfWorkplaceIsNotInDistrict();
		};
		_navMeshListenerEntityRegistry.RegisterNavMeshListener(this);
	}

	public void DeleteEntity()
	{
		_navMeshListenerEntityRegistry.UnregisterNavMeshListener(this);
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		ScheduleWorkplaceReachabilityCheck();
	}

	private void ScheduleWorkplaceReachabilityCheck()
	{
		_checkWorkplaceReachability = true;
	}

	private void UnemployIfWorkplaceIsNotInDistrict()
	{
		if (_worker.Employed)
		{
			DistrictBuilding component = _worker.Workplace.GetComponent<DistrictBuilding>();
			if (!_citizen.HasAssignedDistrict || component.District != _citizen.AssignedDistrict)
			{
				_worker.Unemploy();
			}
		}
	}
}
