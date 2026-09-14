using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.MapEditorPersistence;
using Timberborn.MapMetadataSystem;
using Timberborn.MapRepositorySystem;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using Timberborn.UndoSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapMetadataSystemUI;

internal class MapMetadataPanel : IToolFragment, IPostLoadableSingleton
{
	private class MapMetadataUndoable : IUndoable
	{
		private readonly MapMetadataPanel _metadataPanel;

		private readonly MapMetadata _oldValue;

		private readonly MapMetadata _newValue;

		public MapMetadataUndoable(MapMetadataPanel metadataPanel, MapMetadata oldValue, MapMetadata newValue)
		{
			_metadataPanel = metadataPanel;
			_oldValue = oldValue;
			_newValue = newValue;
		}

		public void Undo()
		{
			_metadataPanel.SetMapMetadata(_oldValue);
			_metadataPanel.OpenToolPanel();
		}

		public void Redo()
		{
			_metadataPanel.SetMapMetadata(_newValue);
			_metadataPanel.OpenToolPanel();
		}
	}

	private readonly DevModeManager _devModeManager;

	private readonly EventBus _eventBus;

	private readonly MapDeserializer _mapDeserializer;

	private readonly MapEditorMapLoader _mapEditorMapLoader;

	private readonly MapMetadataSerializer _mapMetadataSerializer;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly IUndoRegistry _undoRegistry;

	private readonly MapSize _mapSize;

	private readonly ToolService _toolService;

	private readonly MapMetadataSaveEntryWriter _mapMetadataSaveEntryWriter;

	private VisualElement _root;

	private MapMetadataTool _mapMetadataTool;

	private TextField _mapDescription;

	private TextField _mapDescriptionLocKey;

	private TextField _mapNameLocKey;

	private Toggle _isRecommendedToggle;

	private Toggle _isUnconventionalToggle;

	private Toggle _isDevToggle;

	private VisualElement _devControls;

	public MapMetadataPanel(DevModeManager devModeManager, EventBus eventBus, MapDeserializer mapDeserializer, MapEditorMapLoader mapEditorMapLoader, MapMetadataSerializer mapMetadataSerializer, VisualElementLoader visualElementLoader, IUndoRegistry undoRegistry, MapSize mapSize, ToolService toolService, MapMetadataSaveEntryWriter mapMetadataSaveEntryWriter)
	{
		_devModeManager = devModeManager;
		_eventBus = eventBus;
		_mapDeserializer = mapDeserializer;
		_mapEditorMapLoader = mapEditorMapLoader;
		_mapMetadataSerializer = mapMetadataSerializer;
		_visualElementLoader = visualElementLoader;
		_undoRegistry = undoRegistry;
		_mapSize = mapSize;
		_toolService = toolService;
		_mapMetadataSaveEntryWriter = mapMetadataSaveEntryWriter;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/MapMetadataPanel");
		_mapDescription = _root.Q<TextField>("MapDescription");
		_mapDescriptionLocKey = _root.Q<TextField>("MapDescriptionLocKey");
		_mapNameLocKey = _root.Q<TextField>("MapNameLocKey");
		_isRecommendedToggle = _root.Q<Toggle>("IsRecommended");
		_isUnconventionalToggle = _root.Q<Toggle>("IsUnconventional");
		_isDevToggle = _root.Q<Toggle>("IsDev");
		_devControls = _root.Q<VisualElement>("DevControls");
		_root.ToggleDisplayStyle(visible: false);
		_eventBus.Register(this);
		UpdateDevControlsVisibility();
		RegisterOnEvents();
		return _root;
	}

	public void PostLoad()
	{
		SetMapMetadata(GetMapMetadata() ?? GetCurrentMapMetadata());
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		if (toolEnteredEvent.Tool is MapMetadataTool mapMetadataTool)
		{
			_mapMetadataTool = mapMetadataTool;
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		_root.ToggleDisplayStyle(visible: false);
	}

	[OnEvent]
	public void OnDevModeToggled(DevModeToggledEvent devModeToggledEvent)
	{
		UpdateDevControlsVisibility();
	}

	private void RegisterOnEvents()
	{
		_mapDescription.RegisterValueChangedCallback(delegate
		{
			OnValueChanged();
		});
		_mapDescription.isDelayed = true;
		_mapDescriptionLocKey.RegisterValueChangedCallback(delegate
		{
			OnValueChanged();
		});
		_mapDescriptionLocKey.isDelayed = true;
		_mapNameLocKey.RegisterValueChangedCallback(delegate
		{
			OnValueChanged();
		});
		_mapNameLocKey.isDelayed = true;
		_isRecommendedToggle.RegisterValueChangedCallback(delegate
		{
			OnValueChanged();
		});
		_isUnconventionalToggle.RegisterValueChangedCallback(delegate
		{
			OnValueChanged();
		});
		_isDevToggle.RegisterValueChangedCallback(delegate
		{
			OnValueChanged();
		});
	}

	private void OnValueChanged()
	{
		SetMapMetadata(GetCurrentMapMetadata(), registerChange: true);
	}

	private MapMetadata GetCurrentMapMetadata()
	{
		return new MapMetadata(_mapSize.TerrainSize.x, _mapSize.TerrainSize.y, _mapNameLocKey.text, _mapDescriptionLocKey.text, _mapDescription.text, _isRecommendedToggle.value, _isUnconventionalToggle.value, _isDevToggle.value);
	}

	private void UpdateDevControlsVisibility()
	{
		_devControls.ToggleDisplayStyle(_devModeManager.Enabled);
	}

	private void SetMapMetadata(MapMetadata mapMetadata, bool registerChange = false)
	{
		MapMetadata currentMapMetadata = _mapMetadataSaveEntryWriter.CurrentMapMetadata;
		_mapMetadataSaveEntryWriter.SetCurrentMapMetadata(mapMetadata);
		if (registerChange)
		{
			_undoRegistry.RegisterSingleUndoable(new MapMetadataUndoable(this, currentMapMetadata, mapMetadata));
		}
		FillMetadataElements(mapMetadata);
	}

	private void FillMetadataElements(MapMetadata mapMetadata)
	{
		if (mapMetadata != null)
		{
			_mapDescription.SetValueWithoutNotify(mapMetadata.MapDescription);
			_mapDescriptionLocKey.SetValueWithoutNotify(mapMetadata.MapDescriptionLocKey);
			_mapNameLocKey.SetValueWithoutNotify(mapMetadata.MapNameLocKey);
			_isRecommendedToggle.SetValueWithoutNotify(mapMetadata.IsRecommended);
			_isUnconventionalToggle.SetValueWithoutNotify(mapMetadata.IsUnconventional);
			_isDevToggle.SetValueWithoutNotify(mapMetadata.IsDev);
		}
	}

	private void OpenToolPanel()
	{
		_toolService.SwitchTool(_mapMetadataTool);
	}

	private MapMetadata GetMapMetadata()
	{
		if (_mapEditorMapLoader.LoadedMap.HasValue)
		{
			MapFileReference value = _mapEditorMapLoader.LoadedMap.Value;
			return _mapDeserializer.ReadFromMapFileUnsafe(value, _mapMetadataSerializer);
		}
		return null;
	}
}
