using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using Timberborn.TemplateSystem;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using Timberborn.WorkSystem;
using Timberborn.WorkerTypesUI;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

public class WorkerTypeToggle
{
	private static readonly string BeaverClass = "worker-type-toggle__icon--beaver";

	private static readonly string BotClass = "worker-type-toggle__icon--bot";

	private static readonly string WorkplaceUnlockTooltipLocKey = "Work.WorkplaceUnlock.Tooltip";

	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly WorkerTypeHelper _workerTypeHelper;

	private readonly WorkplaceUnlockingDialogService _workplaceUnlockingDialogService;

	private readonly ILoc _loc;

	private SliderToggle _sliderToggle;

	private WorkplaceWorkerType _workplaceWorkerType;

	private WorkplaceSpec _workplaceSpec;

	private bool _botEnabled;

	private bool _beaverEnabled;

	private readonly Phrase _scienceCostPhrase = Phrase.New().FormatCompact();

	public VisualElement Root => _sliderToggle.Root;

	public WorkerTypeToggle(SliderToggleFactory sliderToggleFactory, VisualElementLoader visualElementLoader, WorkerTypeHelper workerTypeHelper, WorkplaceUnlockingDialogService workplaceUnlockingDialogService, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_visualElementLoader = visualElementLoader;
		_workerTypeHelper = workerTypeHelper;
		_workplaceUnlockingDialogService = workplaceUnlockingDialogService;
		_loc = loc;
	}

	public void Initialize(VisualElement parent, string toggleBindingKey = null)
	{
		SliderToggleItem sliderToggleItem = SliderToggleItem.CreateBlockable(GetBeaverButtonTooltip, BeaverClass, SetBeaverWorkerType, GetBeaverToggleState);
		SliderToggleItem sliderToggleItem2 = SliderToggleItem.CreateBlockable(GetBotButtonTooltip, BotClass, SetBotWorkerType, GetBotToggleState);
		_sliderToggle = (string.IsNullOrWhiteSpace(toggleBindingKey) ? _sliderToggleFactory.Create(parent, sliderToggleItem, sliderToggleItem2) : _sliderToggleFactory.CreateBindable(parent, toggleBindingKey, sliderToggleItem, sliderToggleItem2));
	}

	public void Show(WorkplaceWorkerType workplaceWorkerType)
	{
		_workplaceWorkerType = workplaceWorkerType;
		_workplaceSpec = _workplaceWorkerType.GetComponent<WorkplaceSpec>();
		SetEnabledState();
	}

	public void Update()
	{
		if (!_sliderToggle.IsBound)
		{
			_sliderToggle.Bind();
		}
		_sliderToggle.Update();
	}

	public void Clear()
	{
		_sliderToggle.Unbind();
		_sliderToggle.Clear();
		_workplaceWorkerType = null;
		_workplaceSpec = null;
	}

	private void SetBeaverWorkerType()
	{
		_workplaceWorkerType.SetWorkerType(WorkerTypeHelper.BeaverWorkerType);
	}

	private void SetBotWorkerType()
	{
		if (IsBotUnlocked())
		{
			_workplaceWorkerType.SetWorkerType(WorkerTypeHelper.BotWorkerType);
		}
		else
		{
			TryToUnlock();
		}
	}

	private bool IsBotUnlocked()
	{
		UnlockableWorkerType botUnlockableWorkerType = GetBotUnlockableWorkerType();
		return _workplaceUnlockingDialogService.IsWorkerTypeUnlocked(botUnlockableWorkerType);
	}

	private void TryToUnlock()
	{
		UnlockableWorkerType botUnlockableWorkerType = GetBotUnlockableWorkerType();
		_workplaceUnlockingDialogService.TryToUnlockWorkerType(botUnlockableWorkerType, SetBotWorkerType);
	}

	private UnlockableWorkerType GetBotUnlockableWorkerType()
	{
		return new UnlockableWorkerType(_workplaceWorkerType.GetComponent<TemplateSpec>().TemplateName, WorkerTypeHelper.BotWorkerType);
	}

	private void SetEnabledState()
	{
		string defaultWorkerType = _workplaceSpec.DefaultWorkerType;
		bool flag = !_workplaceSpec.DisallowOtherWorkerTypes;
		_botEnabled = flag || _workerTypeHelper.IsBotWorkerType(defaultWorkerType);
		_beaverEnabled = flag || _workerTypeHelper.IsBeaverWorkerType(defaultWorkerType);
	}

	private TooltipContent GetBeaverButtonTooltip()
	{
		if (IsBotUnlocked())
		{
			return TooltipContent.Create((Func<string>)GetBeaverTooltipText);
		}
		return TooltipContent.CreateInstant(GetBotLockedTooltipElement);
	}

	private TooltipContent GetBotButtonTooltip()
	{
		if (IsBotUnlocked())
		{
			return TooltipContent.Create((Func<string>)GetBotTooltipText);
		}
		return TooltipContent.CreateInstant(GetBotLockedTooltipElement);
	}

	private string GetBeaverTooltipText()
	{
		if (_workplaceSpec.DisallowOtherWorkerTypes)
		{
			return _workerTypeHelper.GetDisallowedWorkerText(_workplaceSpec.DefaultWorkerType);
		}
		return _workerTypeHelper.GetBeaverWorkerTypeDisplayText();
	}

	private string GetBotTooltipText()
	{
		if (_workplaceSpec.DisallowOtherWorkerTypes)
		{
			return _workerTypeHelper.GetDisallowedWorkerText(_workplaceSpec.DefaultWorkerType);
		}
		return _workerTypeHelper.GetBotWorkerTypeDisplayText();
	}

	private VisualElement GetBotLockedTooltipElement()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/ScienceCostTooltip");
		visualElement.Q<Label>("TooltipText").text = _loc.T(WorkplaceUnlockTooltipLocKey);
		UnlockableWorkerType botUnlockableWorkerType = GetBotUnlockableWorkerType();
		int workerTypeUnlockCost = _workplaceUnlockingDialogService.GetWorkerTypeUnlockCost(botUnlockableWorkerType);
		visualElement.Q<Label>("ScienceCost").text = _loc.T(_scienceCostPhrase, workerTypeUnlockCost);
		return visualElement;
	}

	private SliderToggleState GetBeaverToggleState()
	{
		if (!_beaverEnabled)
		{
			return SliderToggleState.Unclickable;
		}
		if (!_workerTypeHelper.IsBeaverWorkerType(_workplaceWorkerType.WorkerType))
		{
			return SliderToggleState.None;
		}
		return SliderToggleState.Active;
	}

	private SliderToggleState GetBotToggleState()
	{
		if (!_botEnabled)
		{
			return SliderToggleState.Unclickable;
		}
		if (!IsBotUnlocked())
		{
			return SliderToggleState.Locked;
		}
		if (!_workerTypeHelper.IsBotWorkerType(_workplaceWorkerType.WorkerType))
		{
			return SliderToggleState.None;
		}
		return SliderToggleState.Active;
	}
}
