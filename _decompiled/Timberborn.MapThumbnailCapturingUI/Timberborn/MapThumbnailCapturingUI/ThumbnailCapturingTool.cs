using Timberborn.Localization;
using Timberborn.MapThumbnail;
using Timberborn.MapThumbnailCapturing;
using Timberborn.SingletonSystem;
using Timberborn.ThumbnailCapturing;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;

namespace Timberborn.MapThumbnailCapturingUI;

public class ThumbnailCapturingTool : ITool, IToolDescriptor, IWaterIgnoringTool, ILoadableSingleton, IUpdatableSingleton
{
	private static readonly string TitleLocKey = "MapEditor.ThumbnailCapturing.Title";

	private static readonly string DescriptionLocKey = "MapEditor.ThumbnailCapturing.Description";

	private readonly ILoc _loc;

	private readonly MapThumbnailCameraMover _mapThumbnailCameraMover;

	private readonly ThumbnailRenderer _thumbnailRenderer;

	private readonly EventBus _eventBus;

	private readonly ToolService _toolService;

	private ToolDescription _toolDescription;

	private bool _enabled;

	public ThumbnailCapturingTool(ILoc loc, MapThumbnailCameraMover mapThumbnailCameraMover, ThumbnailRenderer thumbnailRenderer, EventBus eventBus, ToolService toolService)
	{
		_loc = loc;
		_mapThumbnailCameraMover = mapThumbnailCameraMover;
		_thumbnailRenderer = thumbnailRenderer;
		_eventBus = eventBus;
		_toolService = toolService;
	}

	public void Load()
	{
		_toolDescription = new ToolDescription.Builder(_loc.T(TitleLocKey)).AddSection(_loc.T(DescriptionLocKey)).Build();
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		if (_enabled)
		{
			_thumbnailRenderer.Render();
		}
	}

	public void Enter()
	{
		_enabled = true;
	}

	public void Exit()
	{
		_enabled = false;
	}

	public ToolDescription DescribeTool()
	{
		return _toolDescription;
	}

	public void ChangeThumbnail()
	{
		_mapThumbnailCameraMover.MoveToMainCameraPosition();
	}

	public void ResetThumbnail()
	{
		_mapThumbnailCameraMover.MoveToDefaultPosition();
	}

	[OnEvent]
	public void OnMapThumbnailChanged(MapThumbnailChangedEvent mapThumbnailChangedEvent)
	{
		_toolService.SwitchTool(this);
	}
}
