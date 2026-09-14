using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MapEditorSceneLoading;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.MapRepositorySystemUI;

public class NewMapBox : IPanelController, ILoadableSingleton
{
	private static readonly string SizePromptLocKey = "MapEditor.SizePrompt";

	private readonly MapEditorSceneLoader _mapEditorSceneLoader;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly ILoc _loc;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly ISpecService _specService;

	private MapSizeSpec _mapSizeSpec;

	private VisualElement _root;

	private TextField _sizeXField;

	private TextField _sizeYField;

	public NewMapBox(MapEditorSceneLoader mapEditorSceneLoader, DialogBoxShower dialogBoxShower, ILoc loc, VisualElementLoader visualElementLoader, PanelStack panelStack, ISpecService specService)
	{
		_mapEditorSceneLoader = mapEditorSceneLoader;
		_dialogBoxShower = dialogBoxShower;
		_loc = loc;
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_specService = specService;
	}

	public void Load()
	{
		_mapSizeSpec = _specService.GetSingleSpec<MapSizeSpec>();
		_root = _visualElementLoader.LoadVisualElement("Options/NewMapBox");
		_root.Q<Button>("StartButton").RegisterCallback<ClickEvent>(delegate
		{
			StartNewMap();
		});
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_sizeXField = _root.Q<TextField>("SizeXField");
		_sizeYField = _root.Q<TextField>("SizeYField");
	}

	public VisualElement GetPanel()
	{
		_sizeXField.value = _mapSizeSpec.DefaultMapSize.x.ToString();
		_sizeYField.value = _mapSizeSpec.DefaultMapSize.y.ToString();
		return _root;
	}

	public bool OnUIConfirmed()
	{
		StartNewMap();
		return true;
	}

	public void OnUICancelled()
	{
		_panelStack.Pop(this);
	}

	private void StartNewMap()
	{
		if (TryParseSize(_sizeXField.text, out var size) && TryParseSize(_sizeYField.text, out var size2))
		{
			Vector2Int mapSize = new Vector2Int(size, size2);
			_mapEditorSceneLoader.StartNewMap(mapSize);
		}
		else
		{
			_dialogBoxShower.Create().SetMessage(_loc.T(SizePromptLocKey, _mapSizeSpec.MinMapSize, _mapSizeSpec.MaxMapSize)).Show();
		}
	}

	private bool TryParseSize(string text, out int size)
	{
		if (int.TryParse(text, out size) && size >= _mapSizeSpec.MinMapSize)
		{
			return size <= _mapSizeSpec.MaxMapSize;
		}
		return false;
	}
}
