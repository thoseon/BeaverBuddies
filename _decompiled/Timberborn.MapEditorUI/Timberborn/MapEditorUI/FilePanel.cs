using System;
using JetBrains.Annotations;
using Timberborn.CoreUI;
using Timberborn.InputSystemUI;
using Timberborn.Localization;
using Timberborn.MapEditorPersistenceUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using Timberborn.UndoSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorUI;

public class FilePanel : ILoadableSingleton
{
	private static readonly string SaveMapKey = "SaveMap";

	private static readonly string UndoKey = "Undo";

	private static readonly string RedoKey = "Redo";

	private readonly MapSaverLoader _mapSaverLoader;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly ILoc _loc;

	private readonly IUndoRegistry _undoRegistry;

	private readonly EventBus _eventBus;

	private VisualElement _mapFileButtons;

	private BindableButton _undoButton;

	private BindableButton _redoButton;

	public FilePanel(MapSaverLoader mapSaverLoader, VisualElementLoader visualElementLoader, UILayout uiLayout, BindableButtonFactory bindableButtonFactory, ILoc loc, IUndoRegistry undoRegistry, EventBus eventBus)
	{
		_mapSaverLoader = mapSaverLoader;
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_bindableButtonFactory = bindableButtonFactory;
		_loc = loc;
		_undoRegistry = undoRegistry;
		_eventBus = eventBus;
	}

	public void Load()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("MapEditor/FilePanel");
		_mapFileButtons = visualElement.Q<VisualElement>("MapFileButtons");
		_bindableButtonFactory.CreateAndBind(visualElement.Q<Button>("SaveButton"), SaveMapKey, delegate
		{
			_mapSaverLoader.Save();
		});
		visualElement.Q<Button>("SaveAsButton").RegisterCallback<ClickEvent>(delegate
		{
			_mapSaverLoader.SaveAs();
		});
		visualElement.Q<Button>("LoadButton").RegisterCallback<ClickEvent>(delegate
		{
			_mapSaverLoader.LoadMap();
		});
		visualElement.Q<Button>("NewMapButton").RegisterCallback<ClickEvent>(delegate
		{
			_mapSaverLoader.NewMap();
		});
		_undoButton = _bindableButtonFactory.CreateAndBind(visualElement.Q<Button>("UndoButton"), UndoKey, delegate
		{
			_undoRegistry.Undo();
		});
		_redoButton = _bindableButtonFactory.CreateAndBind(visualElement.Q<Button>("RedoButton"), RedoKey, delegate
		{
			_undoRegistry.Redo();
		});
		UpdateUndoButtons();
		_uiLayout.AddTopLeft(visualElement, 1);
		_eventBus.Register(this);
	}

	[UsedImplicitly]
	public void AddMapFileButton(Action action, string locKey)
	{
		Button button = (Button)_visualElementLoader.LoadVisualElement("MapEditor/FilePanelButton");
		button.text = _loc.T(locKey);
		button.RegisterCallback<ClickEvent>(delegate
		{
			action();
		});
		_mapFileButtons.Add(button);
	}

	[OnEvent]
	public void OnUndoStateChanged(UndoStateChangedEvent undoStateChangedEvent)
	{
		UpdateUndoButtons();
	}

	private void UpdateUndoButtons()
	{
		if (_undoRegistry.CanUndo)
		{
			_undoButton.Enable();
		}
		else
		{
			_undoButton.Disable();
		}
		if (_undoRegistry.CanRedo)
		{
			_redoButton.Enable();
		}
		else
		{
			_redoButton.Disable();
		}
	}
}
