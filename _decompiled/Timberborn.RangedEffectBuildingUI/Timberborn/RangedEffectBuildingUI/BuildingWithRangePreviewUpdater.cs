using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingRange;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.RangedEffectBuildingUI;

public class BuildingWithRangePreviewUpdater : BaseComponent, IAwakableComponent, IPreviewSelectionListener, IPostPlacementChangeListener
{
	private readonly BuildingWithRangeUpdateService _buildingWithRangeUpdateService;

	private readonly EventBus _eventBus;

	private BlockObject _blockObject;

	private Preview _preview;

	private IBuildingWithRange _buildingWithRange;

	private bool _isRegistered;

	private bool _drawArea;

	public BuildingWithRangePreviewUpdater(BuildingWithRangeUpdateService buildingWithRangeUpdateService, EventBus eventBus)
	{
		_buildingWithRangeUpdateService = buildingWithRangeUpdateService;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_preview = GetComponent<Preview>();
		_buildingWithRange = GetComponent<IBuildingWithRange>();
	}

	public void OnPreviewSelect()
	{
		RegisterOnPreviewSelect();
		DrawArea();
	}

	public void OnPreviewUnselect()
	{
		_buildingWithRangeUpdateService.DrawArea();
		_drawArea = true;
	}

	public void OnPostPlacementChanged()
	{
		if (_blockObject.IsPreview)
		{
			_drawArea = true;
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		UnregisterOnToolExited();
	}

	private void RegisterOnPreviewSelect()
	{
		if (!_isRegistered)
		{
			_eventBus.Register(this);
			_buildingWithRangeUpdateService.AddPreview(_buildingWithRange, _preview);
			_isRegistered = true;
		}
	}

	private void UnregisterOnToolExited()
	{
		if (_isRegistered)
		{
			_eventBus.Unregister(this);
			_buildingWithRangeUpdateService.RemovePreview();
			_isRegistered = false;
			_drawArea = false;
		}
	}

	private void DrawArea()
	{
		if (_drawArea)
		{
			_buildingWithRangeUpdateService.DrawArea();
			_drawArea = false;
		}
	}
}
