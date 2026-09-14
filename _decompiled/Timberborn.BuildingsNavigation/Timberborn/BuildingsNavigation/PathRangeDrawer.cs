using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.GameDistricts;
using Timberborn.SelectionSystem;

namespace Timberborn.BuildingsNavigation;

internal class PathRangeDrawer : BaseComponent, IAwakableComponent, IUpdatableComponent, ISelectionListener, IPreviewSelectionListener
{
	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private BlockObjectCenter _blockObjectCenter;

	private BlockObject _blockObject;

	private Preview _preview;

	private PathDistrictRetriever _pathDistrictRetriever;

	public PathRangeDrawer(DistrictCenterRegistry districtCenterRegistry)
	{
		_districtCenterRegistry = districtCenterRegistry;
	}

	public void Awake()
	{
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_blockObject = GetComponent<BlockObject>();
		_preview = GetComponent<Preview>();
		_pathDistrictRetriever = GetComponent<PathDistrictRetriever>();
		DisableComponent();
	}

	public void Update()
	{
		DrawRange();
	}

	public void OnSelect()
	{
		DrawRange();
		EnableComponent();
	}

	public void OnUnselect()
	{
		DisableComponent();
	}

	public void OnPreviewSelect()
	{
		if (_preview.PreviewState.IsLast)
		{
			DrawRange(_preview.PreviewState.IsSingle);
		}
	}

	public void OnPreviewUnselect()
	{
	}

	private void DrawRange(bool isSingle = true)
	{
		bool isPreview = _blockObject.IsPreview;
		bool isFinished = _blockObject.IsFinished;
		DistrictCenter districtCenter = GetDistrictCenter(isPreview, isFinished);
		if ((bool)districtCenter)
		{
			DistrictPathNavRangeDrawer component = districtCenter.GetComponent<DistrictPathNavRangeDrawer>();
			DrawingParameters drawingParameters = new DrawingParameters(isPreview || !isFinished, _blockObjectCenter.WorldCenterGrounded, _preview.BlockObject.Orientation, isSingle);
			component.DrawRange(drawingParameters);
		}
	}

	private DistrictCenter GetDistrictCenter(bool isPreview, bool isFinished)
	{
		if (isFinished)
		{
			return _pathDistrictRetriever.GetFinishedDistrictCenter();
		}
		if (!isPreview || _preview.PreviewState.IsBuildable)
		{
			return GetDistrictCenter();
		}
		return null;
	}

	private DistrictCenter GetDistrictCenter()
	{
		foreach (DistrictCenter allDistrictCenter in _districtCenterRegistry.AllDistrictCenters)
		{
			if (allDistrictCenter.IsOnPreviewDistrictRoad(_blockObjectCenter.WorldCenterGrounded))
			{
				return allDistrictCenter;
			}
		}
		return null;
	}
}
