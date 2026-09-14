using Timberborn.BaseComponentSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.BuilderPrioritySystemUI;
using Timberborn.CoreUI;
using Timberborn.Demolishing;
using Timberborn.EntityPanelSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.PrioritySystemUI;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.DemolishingUI;

internal class DemolishableFragment : IEntityPanelFragment, IInputProcessor
{
	private static readonly string MarkLocKey = "Demolish.Mark";

	private static readonly string CancelLocKey = "Demolish.Cancel";

	private static readonly string PriorityLabelLocKey = "Demolish.PriorityTitle";

	private static readonly string UniqueBuildingActionKey = "UniqueBuildingAction";

	private readonly DemolishableScienceRewardLabelFactory _demolishableScienceRewardLabelFactory;

	private readonly BuilderPriorityToggleGroupFactory _builderPriorityToggleGroupFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly InputService _inputService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private Demolishable _demolishable;

	private VisualElement _root;

	private VisualElement _buttonWrapper;

	private Button _button;

	private PriorityToggleGroup _priorityToggleGroup;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	private Label _progressLabel;

	private VisualElement _hidable;

	private VisualElement _scienceRewardWrapper;

	private DemolishableScienceRewardLabel _demolishableScienceRewardLabel;

	private readonly Phrase _progressPhrase = Phrase.New().FormatPercentCeiled();

	public DemolishableFragment(DemolishableScienceRewardLabelFactory demolishableScienceRewardLabelFactory, BuilderPriorityToggleGroupFactory builderPriorityToggleGroupFactory, VisualElementLoader visualElementLoader, ILoc loc, InputService inputService, ITooltipRegistrar tooltipRegistrar)
	{
		_demolishableScienceRewardLabelFactory = demolishableScienceRewardLabelFactory;
		_builderPriorityToggleGroupFactory = builderPriorityToggleGroupFactory;
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_inputService = inputService;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DemolishableFragment");
		_buttonWrapper = _root.Q<VisualElement>("ButtonWrapper");
		_button = _root.Q<Button>("Button");
		_button.RegisterCallback<ClickEvent>(delegate
		{
			ChangeDemolishState();
		});
		VisualElement parent = _root.Q<VisualElement>("PriorityWrapper");
		_priorityToggleGroup = _builderPriorityToggleGroupFactory.Create(parent, PriorityLabelLocKey);
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_progressLabel = _root.Q<Label>("Progress");
		_hidable = _root.Q<VisualElement>("HidableWrapper");
		_demolishableScienceRewardLabel = _demolishableScienceRewardLabelFactory.Create();
		_scienceRewardWrapper = _root.Q<VisualElement>("ScienceRewardWrapper");
		_scienceRewardWrapper.Add(_demolishableScienceRewardLabel.Root);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		Demolishable component = entity.GetComponent<Demolishable>();
		if ((bool)component && (component.IsMarked || component.ShowDemolishButtonInEntityPanel))
		{
			_demolishable = component;
			_root.ToggleDisplayStyle(visible: true);
			_priorityToggleGroup.Enable(entity.GetComponent<BuilderPrioritizable>());
			DemolishableScienceRewardSpec component2 = component.GetComponent<DemolishableScienceRewardSpec>();
			_demolishableScienceRewardLabel.Show(component2);
			_scienceRewardWrapper.ToggleDisplayStyle(component2 != null);
			if (_demolishable.ShowDemolishButtonInEntityPanel)
			{
				_inputService.AddInputProcessor(this);
			}
			_buttonWrapper.ToggleDisplayStyle(_demolishable.ShowDemolishButtonInEntityPanel);
		}
	}

	public void ClearFragment()
	{
		_demolishable = null;
		_priorityToggleGroup.Disable();
		_scienceRewardWrapper.ToggleDisplayStyle(visible: false);
		_root.ToggleDisplayStyle(visible: false);
		_inputService.RemoveInputProcessor(this);
	}

	public void UpdateFragment()
	{
		if ((bool)_demolishable)
		{
			string text = (_demolishable.IsMarked ? _loc.T(CancelLocKey) : _loc.T(MarkLocKey));
			_button.text = text;
			_tooltipRegistrar.RegisterWithKeyBinding(_button, text, UniqueBuildingActionKey);
			if (_demolishable.IsMarked)
			{
				_priorityToggleGroup.UpdateGroup();
				float num = Mathf.Clamp01(_demolishable.DemolishingProgress);
				_progressBar.SetProgress(num);
				_progressLabel.text = _loc.T(_progressPhrase, num);
				_hidable.ToggleDisplayStyle(visible: true);
			}
			else
			{
				_hidable.ToggleDisplayStyle(visible: false);
			}
		}
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(UniqueBuildingActionKey))
		{
			ChangeDemolishState();
			return true;
		}
		return false;
	}

	private void ChangeDemolishState()
	{
		if (_demolishable.IsMarked)
		{
			_demolishable.Unmark();
		}
		else
		{
			_demolishable.Mark();
		}
	}
}
