using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.GameDistricts;

public class DistrictCenter : BaseComponent, IAwakableComponent, IRegisteredComponent, IUnfinishedStateListener, IFinishedStateListener
{
	private readonly IDistrictService _districtService;

	private BlockObject _blockObject;

	private BlockObjectCenter _blockObjectCenter;

	private NamedEntity _namedEntity;

	public District District { get; private set; }

	public DistrictPopulation DistrictPopulation { get; private set; }

	public DistrictBuildingRegistry DistrictBuildingRegistry { get; private set; }

	public string DistrictName => _namedEntity.EntityName;

	public Vector3Int CenterCoordinates => _blockObject.PositionedEntrance.DoorstepCoordinates;

	public DistrictCenter(IDistrictService districtService)
	{
		_districtService = districtService;
	}

	public void Awake()
	{
		DistrictPopulation = GetComponent<DistrictPopulation>();
		DistrictBuildingRegistry = GetComponent<DistrictBuildingRegistry>();
		_blockObject = GetComponent<BlockObject>();
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_namedEntity = GetComponent<NamedEntity>();
		DisableComponent();
	}

	public bool AccessibleIsOnDistrictRoad(Accessible accessible)
	{
		if (District != null)
		{
			Vector3? unblockedSingleAccess = accessible.UnblockedSingleAccess;
			if (unblockedSingleAccess.HasValue)
			{
				Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
				return _districtService.IsOnDistrictRoad(District, valueOrDefault);
			}
		}
		return false;
	}

	public bool AccessibleIsOnInstantDistrictRoad(Accessible accessible)
	{
		if (District != null)
		{
			Vector3? unblockedSingleAccessInstant = accessible.UnblockedSingleAccessInstant;
			if (unblockedSingleAccessInstant.HasValue)
			{
				Vector3 valueOrDefault = unblockedSingleAccessInstant.GetValueOrDefault();
				return _districtService.IsOnInstantDistrictRoad(District, valueOrDefault);
			}
		}
		return false;
	}

	public bool IsOnInstantDistrictRoad(Vector3 start)
	{
		if (District != null)
		{
			return _districtService.IsOnInstantDistrictRoad(District, start);
		}
		return false;
	}

	public bool IsOnPreviewDistrictRoad(Vector3 start)
	{
		if (District != null)
		{
			return _districtService.IsOnPreviewDistrictRoad(District, start);
		}
		return false;
	}

	public bool IsGloballyReachableFromCitizen(Citizen citizen)
	{
		if (!citizen.GetComponent<ICitizenPositionOverrider>().TryGetOverridenPosition(out var position))
		{
			position = citizen.Transform.position;
		}
		return IsGloballyReachableFromPosition(position);
	}

	public bool IsGloballyReachableFromAnotherDistrict(DistrictCenter other)
	{
		Vector3 position = NavigationCoordinateSystem.GridToWorld(other.CenterCoordinates);
		return IsGloballyReachableFromPosition(position);
	}

	public float DistanceToCitizen(Citizen citizen)
	{
		return Vector3.Distance(_blockObjectCenter.WorldCenterGrounded, citizen.Transform.position);
	}

	public void OnEnterUnfinishedState()
	{
		District = _districtService.AddPreviewDistrict(CenterCoordinates);
	}

	public void OnExitUnfinishedState()
	{
		_districtService.RemovePreviewDistrict(District);
	}

	public void OnEnterFinishedState()
	{
		District = _districtService.AddDistrict(CenterCoordinates);
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		_districtService.RemoveDistrict(District);
		District = null;
		DisableComponent();
	}

	private bool IsGloballyReachableFromPosition(Vector3 position)
	{
		return _districtService.DistrictIsGloballyReachable(District, position);
	}
}
