using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectAccesses;
using Timberborn.BlockSystem;
using Timberborn.BuildingsReachability;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.MapStateSystem;
using Timberborn.Navigation;

namespace Timberborn.RecoveredGoodSystem;

internal class RecoveredGoodStackAccessible : BaseComponent, IAwakableComponent, INavMeshListener, IInitializableEntity, IDeletableEntity, IUnreachableEntity, IAccessibleNeeder
{
	private readonly IDistrictService _districtService;

	private readonly INavMeshListenerEntityRegistry _navMeshListenerEntityRegistry;

	private readonly MapSize _mapSize;

	private BlockObject _blockObject;

	private BlockObjectAccessGenerator _blockObjectAccessGenerator;

	private BoundingBox _bounds;

	public Accessible Accessible { get; private set; }

	public string AccessibleComponentName => "RecoveredGoodStack";

	private int MinZ => _blockObject.CoordinatesAtBaseZ.z - 1;

	private int MaxZ => _mapSize.TotalSize.z - 1;

	public RecoveredGoodStackAccessible(IDistrictService districtService, INavMeshListenerEntityRegistry navMeshListenerEntityRegistry, MapSize mapSize)
	{
		_districtService = districtService;
		_navMeshListenerEntityRegistry = navMeshListenerEntityRegistry;
		_mapSize = mapSize;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectAccessGenerator = GetComponent<BlockObjectAccessGenerator>();
	}

	public void SetAccessible(Accessible accessible)
	{
		Accessible = accessible;
	}

	public void InitializeEntity()
	{
		_navMeshListenerEntityRegistry.RegisterNavMeshListener(this);
		UpdateAccesses();
	}

	public void DeleteEntity()
	{
		_navMeshListenerEntityRegistry.UnregisterNavMeshListener(this);
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		if (_bounds.Intersects(navMeshUpdate.Bounds))
		{
			UpdateAccesses();
		}
	}

	public bool IsUnreachable()
	{
		return !_districtService.IsOnInstantDistrictRoadSpill(Accessible);
	}

	public void UpdateAccesses()
	{
		_bounds = _blockObjectAccessGenerator.GenerateAccessBounds(MinZ, MaxZ);
		Accessible.SetAccesses(_blockObjectAccessGenerator.GenerateAccesses(MinZ, MaxZ));
	}
}
