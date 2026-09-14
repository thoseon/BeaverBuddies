using Timberborn.AreaSelectionSystem;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlueprintSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.PrioritySystem;
using Timberborn.ToolButtonSystem;
using Timberborn.UISound;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.BuilderPrioritySystemUI;

internal class BuilderPrioritiesButtonFactory
{
	private readonly BlockObjectSelectionDrawerFactory _blockObjectSelectionDrawerFactory;

	private readonly BuilderPrioritizableHighlighter _builderPrioritizableHighlighter;

	private readonly AreaBlockObjectPickerFactory _areaBlockObjectPickerFactory;

	private readonly BuilderPrioritySpriteLoader _builderPrioritySpriteLoader;

	private readonly UISoundController _uiSoundController;

	private readonly ToolButtonFactory _toolButtonFactory;

	private readonly CursorService _cursorService;

	private readonly InputService _inputService;

	private readonly ISpecService _specService;

	private readonly ILoc _loc;

	public BuilderPrioritiesButtonFactory(BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory, BuilderPrioritizableHighlighter builderPrioritizableHighlighter, AreaBlockObjectPickerFactory areaBlockObjectPickerFactory, BuilderPrioritySpriteLoader builderPrioritySpriteLoader, UISoundController uiSoundController, ToolButtonFactory toolButtonFactory, CursorService cursorService, InputService inputService, ISpecService specService, ILoc loc)
	{
		_blockObjectSelectionDrawerFactory = blockObjectSelectionDrawerFactory;
		_builderPrioritizableHighlighter = builderPrioritizableHighlighter;
		_areaBlockObjectPickerFactory = areaBlockObjectPickerFactory;
		_builderPrioritySpriteLoader = builderPrioritySpriteLoader;
		_uiSoundController = uiSoundController;
		_toolButtonFactory = toolButtonFactory;
		_cursorService = cursorService;
		_inputService = inputService;
		_specService = specService;
		_loc = loc;
	}

	public ToolButton CreateButton(Priority priority, VisualElement parent)
	{
		BuilderPriorityTool tool = CreateTool(priority);
		Sprite toolImage = _builderPrioritySpriteLoader.LoadButtonSprite(priority);
		return _toolButtonFactory.Create(tool, toolImage, parent);
	}

	private BuilderPriorityTool CreateTool(Priority priority)
	{
		BuilderPriorityTool builderPriorityTool = new BuilderPriorityTool(_areaBlockObjectPickerFactory, _inputService, _blockObjectSelectionDrawerFactory, _cursorService, _loc, _builderPrioritizableHighlighter, _uiSoundController);
		BuilderPriorityToolSpec singleSpec = _specService.GetSingleSpec<BuilderPriorityToolSpec>();
		builderPriorityTool.Initialize(priority, singleSpec);
		return builderPriorityTool;
	}
}
