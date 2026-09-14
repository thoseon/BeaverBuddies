using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingsReachability;

namespace Timberborn.LinkedBuildingSystem;

internal class LinkedConstructionSiteReachability : BaseComponent, IAwakableComponent, IExpandedConstructionSiteReachability
{
	private readonly PreviewBlockService _previewBlockService;

	private BlockObject _blockObject;

	private ReachableConstructionSite _reachableConstructionSite;

	private LinkedConstructionSiteReachability _linked;

	private readonly MirrorOperationLock _mirrorOperationLock = new MirrorOperationLock();

	public LinkedConstructionSiteReachability(PreviewBlockService previewBlockService)
	{
		_previewBlockService = previewBlockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_reachableConstructionSite = GetComponent<ReachableConstructionSite>();
		GetComponent<LinkedBuilding>().BuildingLinked += OnBuildingLinked;
	}

	public bool IsReachable()
	{
		if (_mirrorOperationLock.IsUnlocked)
		{
			if (!_blockObject.IsPreview)
			{
				if ((bool)_linked)
				{
					return _linked.IsReachableWithoutMirroring();
				}
				return false;
			}
			return IsPreviewReachable();
		}
		return false;
	}

	private void OnBuildingLinked(object sender, LinkedBuilding e)
	{
		_linked = e.GetComponent<LinkedConstructionSiteReachability>();
	}

	private bool IsPreviewReachable()
	{
		return _previewBlockService.GetBottomObjectComponentAt<LinkedConstructionSiteReachability>(_blockObject.CoordinatesBehind()).IsReachableWithoutMirroring();
	}

	private bool IsReachableWithoutMirroring()
	{
		using (_mirrorOperationLock.Lock())
		{
			return _reachableConstructionSite.IsReachableByBuilders();
		}
	}
}
