using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectAccesses;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.MapStateSystem;
using Timberborn.Navigation;

namespace Timberborn.BuildingsNavigation;

public class ConstructionSiteAccessible : BaseComponent, IAwakableComponent, IUnfinishedStateListener, IPreviewSelectionListener, INavMeshListener, IAccessibleNeeder
{
	private readonly INavMeshListenerEntityRegistry _navMeshListenerEntityRegistry;

	private readonly MapSize _mapSize;

	private BlockObject _blockObject;

	private BlockObjectAccessGenerator _blockObjectAccessGenerator;

	private Preview _preview;

	private BoundingBox _bounds;

	private IConstructionSiteAccessProvider _constructionSiteAccessProvider;

	public Accessible Accessible { get; private set; }

	public string AccessibleComponentName => "ConstructionSite";

	private int MinZ => _blockObject.CoordinatesAtBaseZ.z - 1;

	private int MaxZ => _mapSize.TotalSize.z - 1;

	public ConstructionSiteAccessible(INavMeshListenerEntityRegistry navMeshListenerEntityRegistry, MapSize mapSize)
	{
		_navMeshListenerEntityRegistry = navMeshListenerEntityRegistry;
		_mapSize = mapSize;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectAccessGenerator = GetComponent<BlockObjectAccessGenerator>();
		_preview = GetComponent<Preview>();
		_constructionSiteAccessProvider = GetComponent<IConstructionSiteAccessProvider>();
		DisableComponent();
	}

	public void SetAccessible(Accessible accessible)
	{
		Accessible = accessible;
	}

	public void OnEnterUnfinishedState()
	{
		UpdateAccesses();
		_bounds = _blockObjectAccessGenerator.GenerateAccessBounds(MinZ, MaxZ);
		_navMeshListenerEntityRegistry.RegisterNavMeshListener(this);
		EnableComponent();
	}

	public void OnExitUnfinishedState()
	{
		DisableAccesses();
		_navMeshListenerEntityRegistry.UnregisterNavMeshListener(this);
		DisableComponent();
	}

	public void OnPreviewSelect()
	{
		if (_preview.PreviewState.IsSingle)
		{
			UpdateAccesses();
		}
		else
		{
			DisableAccesses();
		}
	}

	public void OnPreviewUnselect()
	{
		DisableAccesses();
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		if (_bounds.Intersects(navMeshUpdate.Bounds))
		{
			UpdateAccesses();
		}
	}

	private void UpdateAccesses()
	{
		Accessible.SetAccesses((_constructionSiteAccessProvider != null) ? _constructionSiteAccessProvider.GetAccesses() : _blockObjectAccessGenerator.GenerateAccesses(MinZ, MaxZ));
	}

	private void DisableAccesses()
	{
		Accessible.ClearAccesses();
	}
}
