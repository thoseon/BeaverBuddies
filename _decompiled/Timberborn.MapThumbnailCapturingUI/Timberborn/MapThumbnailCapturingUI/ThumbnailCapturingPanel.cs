using Timberborn.CoreUI;
using Timberborn.FileBrowsing;
using Timberborn.MapThumbnail;
using Timberborn.MapThumbnailOverlaySystem;
using Timberborn.SingletonSystem;
using Timberborn.ThumbnailCapturing;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapThumbnailCapturingUI;

internal class ThumbnailCapturingPanel : IToolFragment, IPostLoadableSingleton
{
	private static readonly string OverlayTipLocKey = "MapEditor.ThumbnailCapturing.OverlayTip";

	private readonly EventBus _eventBus;

	private readonly IThumbnailRenderTextureProvider _thumbnailRenderTextureProvider;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly FileBrowser _fileBrowser;

	private readonly ToolService _toolService;

	private readonly MapThumbnailOverlay _mapThumbnailOverlay;

	private readonly FileFilterProvider _fileFilterProvider;

	private VisualElement _root;

	private ThumbnailCapturingTool _thumbnailCapturingTool;

	private Image _overlayImage;

	private Button _clearButton;

	public ThumbnailCapturingPanel(EventBus eventBus, IThumbnailRenderTextureProvider thumbnailRenderTextureProvider, VisualElementLoader visualElementLoader, FileBrowser fileBrowser, ToolService toolService, MapThumbnailOverlay mapThumbnailOverlay, FileFilterProvider fileFilterProvider)
	{
		_eventBus = eventBus;
		_thumbnailRenderTextureProvider = thumbnailRenderTextureProvider;
		_visualElementLoader = visualElementLoader;
		_fileBrowser = fileBrowser;
		_toolService = toolService;
		_mapThumbnailOverlay = mapThumbnailOverlay;
		_fileFilterProvider = fileFilterProvider;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/ThumbnailCapturingPanel");
		_root.Q<Button>("Update").RegisterCallback<ClickEvent>(delegate
		{
			_thumbnailCapturingTool.ChangeThumbnail();
		});
		_root.Q<Button>("Reset").RegisterCallback<ClickEvent>(delegate
		{
			_thumbnailCapturingTool.ResetThumbnail();
		});
		_root.Q<Button>("SelectOverlay").RegisterCallback<ClickEvent>(SelectOverlay);
		_clearButton = _root.Q<Button>("ClearOverlay");
		_clearButton.RegisterCallback<ClickEvent>(ClearOverlay);
		_overlayImage = _root.Q<Image>("Overlay");
		_root.ToggleDisplayStyle(visible: false);
		_eventBus.Register(this);
		return _root;
	}

	public void PostLoad()
	{
		_root.Q<Image>("Preview").image = _thumbnailRenderTextureProvider.RenderTexture;
		UpdateOverlayImage();
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		_thumbnailCapturingTool = toolEnteredEvent.Tool as ThumbnailCapturingTool;
		if (_thumbnailCapturingTool != null)
		{
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		_root.ToggleDisplayStyle(visible: false);
	}

	[OnEvent]
	public void OnPanelHidden(PanelHiddenEvent panelHiddenEvent)
	{
		if (!panelHiddenEvent.AnyPanelShown && _toolService.ActiveTool is ThumbnailCapturingTool)
		{
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	[OnEvent]
	public void OnMapThumbnailChanged(MapThumbnailChangedEvent mapThumbnailChangedEvent)
	{
		UpdateOverlayImage();
	}

	private void SelectOverlay(ClickEvent evt)
	{
		_root.ToggleDisplayStyle(visible: false);
		_fileBrowser.Open(OverlayChosenCallback, _fileFilterProvider.Images, OverlayTipLocKey);
	}

	private void OverlayChosenCallback(string path)
	{
		_mapThumbnailOverlay.LoadFromFile(path);
		UpdateOverlayImage();
	}

	private void ClearOverlay(ClickEvent evt)
	{
		_mapThumbnailOverlay.Clear();
		UpdateOverlayImage();
	}

	private void UpdateOverlayImage()
	{
		_overlayImage.image = _mapThumbnailOverlay.Overlay;
		_clearButton.ToggleDisplayStyle(_mapThumbnailOverlay.Overlay);
	}
}
