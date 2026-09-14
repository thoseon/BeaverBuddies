using Timberborn.CoreUI;
using Timberborn.InputSystemUI;
using Timberborn.LevelVisibilitySystem;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.LevelVisibilitySystemUI;

internal class LevelVisibilityPanel : ILevelVisibilityPanel, ILoadableSingleton
{
	private static readonly string HeldLevelButtonClass = "level-visibility-panel__level-button--held";

	private static readonly string LevelVisibilityPanelGameClass = "level-visibility-panel--game";

	private static readonly string LevelVisibilityPanelEditorClass = "level-visibility-panel--map-editor";

	private static readonly string MaxLevelBackgroundClass = "square-large--transparent-purple";

	private static readonly string NotMaxLevelBackgroundClass = "square-large--light-red";

	private static readonly string HighlightClass = "highlight";

	private static readonly string RaiseVisibleLayerKey = "RaiseVisibleLayer";

	private static readonly string LowerVisibleLayerKey = "LowerVisibleLayer";

	private static readonly string InfoTooltipKey = "LevelVisibility.InfoTooltip";

	private static readonly string ResetTooltipKey = "LevelVisibility.ResetTooltip";

	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly LevelVisibilitySelector _levelVisibilitySelector;

	private readonly EventBus _eventBus;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly MapEditorMode _mapEditorMode;

	private VisualElement _root;

	private VisualElement _content;

	private BindableButton _upButton;

	private BindableButton _downButton;

	private VisualElement _levelButtonWrapper;

	private RepeatButton _levelButton;

	private Button _resetButton;

	private Label _level;

	private bool _mouseOnLevelButton;

	public LevelVisibilityPanel(UILayout uiLayout, VisualElementLoader visualElementLoader, ILevelVisibilityService levelVisibilityService, LevelVisibilitySelector levelVisibilitySelector, EventBus eventBus, BindableButtonFactory bindableButtonFactory, ITooltipRegistrar tooltipRegistrar, MapEditorMode mapEditorMode)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_levelVisibilityService = levelVisibilityService;
		_levelVisibilitySelector = levelVisibilitySelector;
		_eventBus = eventBus;
		_bindableButtonFactory = bindableButtonFactory;
		_tooltipRegistrar = tooltipRegistrar;
		_mapEditorMode = mapEditorMode;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/LevelVisibilityPanel");
		_content = _root.Q<VisualElement>("Content");
		Button button = _root.Q<Button>("Up");
		Button button2 = _root.Q<Button>("Down");
		_upButton = _bindableButtonFactory.CreateAndBind(button, RaiseVisibleLayerKey, delegate
		{
			ChangeMaxVisibleLevel(1);
		});
		_downButton = _bindableButtonFactory.CreateAndBind(button2, LowerVisibleLayerKey, delegate
		{
			ChangeMaxVisibleLevel(-1);
		});
		_tooltipRegistrar.RegisterWithKeyBinding(button, RaiseVisibleLayerKey);
		_tooltipRegistrar.RegisterWithKeyBinding(button2, LowerVisibleLayerKey);
		_resetButton = _root.Q<Button>("Reset");
		_resetButton.RegisterCallback<ClickEvent>(delegate
		{
			_levelVisibilityService.ResetMaxVisibleLevel();
		});
		_tooltipRegistrar.RegisterLocalizable(_resetButton, ResetTooltipKey);
		_levelButtonWrapper = _root.Q<VisualElement>("LevelButtonWrapper");
		_tooltipRegistrar.RegisterLocalizable(_levelButtonWrapper, InfoTooltipKey);
		_tooltipRegistrar.RegisterLocalizable(_root.Q<VisualElement>("LevelIcon"), InfoTooltipKey);
		_levelButton = _root.Q<RepeatButton>("LevelButton");
		_levelButton.SetAction(StartLevelSelection, 0L, long.MaxValue);
		_level = _root.Q<Label>("Level");
		UpdateButtons();
		_eventBus.Register(this);
		if (_mapEditorMode.IsMapEditor)
		{
			_root.AddToClassList(LevelVisibilityPanelEditorClass);
		}
		else
		{
			_root.AddToClassList(LevelVisibilityPanelGameClass);
		}
	}

	public void TogglePanelHighlight(bool state)
	{
		_root.EnableInClassList(HighlightClass, state);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopRight(_root, _mapEditorMode.IsMapEditor ? 2 : 4);
	}

	[OnEvent]
	public void OnMaxVisibleLevelChanged(MaxVisibleLevelChangedEvent maxVisibleLevelChangedEvent)
	{
		UpdateButtons();
	}

	private void StartLevelSelection()
	{
		_levelVisibilitySelector.StartLevelSelection(ChangeMaxVisibleLevel, EndLevelSelection);
		_levelButton.AddToClassList(HeldLevelButtonClass);
		_levelButtonWrapper.AddToClassList(HeldLevelButtonClass);
	}

	private void EndLevelSelection()
	{
		_levelButton.RemoveFromClassList(HeldLevelButtonClass);
		_levelButtonWrapper.RemoveFromClassList(HeldLevelButtonClass);
	}

	private void ChangeMaxVisibleLevel(int change)
	{
		_levelVisibilityService.SetMaxVisibleLevel(_levelVisibilityService.MaxVisibleLevel + change);
	}

	private void UpdateButtons()
	{
		bool levelIsAtMax = _levelVisibilityService.LevelIsAtMax;
		bool levelIsAtMin = _levelVisibilityService.LevelIsAtMin;
		_level.text = (levelIsAtMax ? "∞" : _levelVisibilityService.MaxVisibleLevel.ToString());
		_resetButton.SetEnabled(!levelIsAtMax);
		_content.EnableInClassList(MaxLevelBackgroundClass, levelIsAtMax);
		_content.EnableInClassList(NotMaxLevelBackgroundClass, !levelIsAtMax);
		if (levelIsAtMax)
		{
			_upButton.Disable();
		}
		else
		{
			_upButton.Enable();
		}
		if (levelIsAtMin)
		{
			_downButton.Disable();
		}
		else
		{
			_downButton.Enable();
		}
		bool enabled = !levelIsAtMin || !levelIsAtMax;
		_levelButton.SetEnabled(enabled);
		_levelButtonWrapper.SetEnabled(enabled);
	}
}
