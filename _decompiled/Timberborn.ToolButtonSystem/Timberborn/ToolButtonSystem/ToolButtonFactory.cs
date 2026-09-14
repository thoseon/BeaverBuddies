using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Timberborn.AssetSystem;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.ToolButtonSystem;

public class ToolButtonFactory
{
	private static readonly string ImageDirectory = "Sprites/BottomBar";

	private readonly EventBus _eventBus;

	private readonly ToolService _toolService;

	private readonly DevModeManager _devModeManager;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ToolButtonService _toolButtonService;

	private readonly IAssetLoader _assetLoader;

	private readonly ToolGroupService _toolGroupService;

	private readonly MapEditorMode _mapEditorMode;

	private readonly ImmutableArray<IToolDisabler> _toolDisablers;

	public ToolButtonFactory(EventBus eventBus, ToolService toolService, DevModeManager devModeManager, VisualElementLoader visualElementLoader, ToolButtonService toolButtonService, IAssetLoader assetLoader, ToolGroupService toolGroupService, MapEditorMode mapEditorMode, IEnumerable<IToolDisabler> toolDisablers)
	{
		_eventBus = eventBus;
		_toolService = toolService;
		_devModeManager = devModeManager;
		_visualElementLoader = visualElementLoader;
		_toolButtonService = toolButtonService;
		_assetLoader = assetLoader;
		_toolGroupService = toolGroupService;
		_mapEditorMode = mapEditorMode;
		_toolDisablers = toolDisablers.ToImmutableArray();
	}

	public ToolButton Create(ITool tool, string imageName, VisualElement parent)
	{
		Sprite toolImage = _assetLoader.Load<Sprite>(Path.Combine(ImageDirectory, imageName));
		return Create(tool, toolImage, parent);
	}

	public ToolButton Create(ITool tool, Sprite toolImage, VisualElement parent)
	{
		VisualElement content = _visualElementLoader.LoadVisualElement("Common/BottomBar/ToolButton");
		return Create(tool, toolImage, parent, content);
	}

	public ToolButton CreateHex(ITool tool, Sprite toolImage, VisualElement parent)
	{
		VisualElement content = _visualElementLoader.LoadVisualElement("Common/BottomBar/ToolButtonHex");
		return Create(tool, toolImage, parent, content);
	}

	public ToolButton CreateGrouplessRed(ITool tool, string imageName)
	{
		Sprite toolImage = _assetLoader.Load<Sprite>(Path.Combine(ImageDirectory, imageName));
		return CreateGroupless(tool, toolImage, "bottom-bar-button--red");
	}

	public ToolButton CreateGrouplessBlue(ITool tool, string imageName)
	{
		Sprite toolImage = _assetLoader.Load<Sprite>(Path.Combine(ImageDirectory, imageName));
		return CreateGroupless(tool, toolImage, "bottom-bar-button--blue");
	}

	public ToolButton CreateGrouplessGreen(ITool tool, Sprite toolImage)
	{
		return CreateGroupless(tool, toolImage, "bottom-bar-button--green");
	}

	private ToolButton CreateGroupless(ITool tool, Sprite toolImage, string className)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/BottomBar/GrouplessToolButton");
		visualElement.Q("Tooltip").ToggleDisplayStyle(visible: false);
		visualElement.AddToClassList(className);
		return CreateButton(tool, toolImage, visualElement);
	}

	private ToolButton Create(ITool tool, Sprite toolImage, VisualElement parent, VisualElement content)
	{
		ToolButton toolButton = CreateButton(tool, toolImage, content);
		parent.Add(toolButton.Root);
		return toolButton;
	}

	private ToolButton CreateButton(ITool tool, Sprite toolImage, VisualElement root)
	{
		root.Q<VisualElement>("ToolImage").style.backgroundImage = new StyleBackground(toolImage);
		ToolButton toolButton = new ToolButton(_toolService, _devModeManager, _toolGroupService, _mapEditorMode, _toolDisablers, tool, root, root.Q<Button>());
		_eventBus.Register(toolButton);
		_toolButtonService.Add(toolButton);
		return toolButton;
	}
}
