using System;
using Timberborn.Automation;
using Timberborn.DropdownSystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationUI;

public class TransmitterSelectorInitializer
{
	private static readonly string AutomationNoneLocKey = "Automation.AutomationNone";

	private static readonly string AutomationOptionalLocKey = "Automation.AutomationOptional";

	private static readonly string AutomateLocKey = "Automation.Automate";

	private static readonly string RemoveRowLocKey = "Automation.RemoveRow";

	private readonly AutomatorRegistry _automatorRegistry;

	private readonly EventBus _eventBus;

	private readonly ILoc _loc;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly AutomationStateIconBuilder _automationStateIconBuilder;

	private readonly TransmitterPickerTool _transmitterPickerTool;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	public TransmitterSelectorInitializer(AutomatorRegistry automatorRegistry, EventBus eventBus, ILoc loc, DropdownItemsSetter dropdownItemsSetter, AutomationStateIconBuilder automationStateIconBuilder, TransmitterPickerTool transmitterPickerTool, ITooltipRegistrar tooltipRegistrar)
	{
		_automatorRegistry = automatorRegistry;
		_eventBus = eventBus;
		_loc = loc;
		_dropdownItemsSetter = dropdownItemsSetter;
		_automationStateIconBuilder = automationStateIconBuilder;
		_transmitterPickerTool = transmitterPickerTool;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public void Initialize(TransmitterSelector transmitterSelector, Func<Automator> getter, Action<Automator> setter, Action closeIconClicked = null)
	{
		InitializeInternal(transmitterSelector, getter, setter, AutomationNoneLocKey, AutomationNoneLocKey, closeIconClicked);
	}

	public void InitializeOptional(TransmitterSelector transmitterSelector, Func<Automator> getter, Action<Automator> setter)
	{
		InitializeInternal(transmitterSelector, getter, setter, AutomationNoneLocKey, AutomationOptionalLocKey);
	}

	public void InitializeStandalone(TransmitterSelector transmitterSelector, Func<Automator> getter, Action<Automator> setter)
	{
		InitializeInternal(transmitterSelector, getter, setter, AutomationNoneLocKey, AutomateLocKey);
	}

	private void InitializeInternal(TransmitterSelector transmitterSelector, Func<Automator> getter, Action<Automator> setter, string noneLocKey, string selectedNoneLocKey, Action closeIconClicked = null)
	{
		TransmitterDropdownProvider transmitterDropdownProvider = new TransmitterDropdownProvider(_automatorRegistry, _loc, getter, setter, noneLocKey, selectedNoneLocKey);
		AutomationStateIcon automationStateIcon = _automationStateIconBuilder.Create(transmitterSelector.Q<Image>("StateIcon"), getter).SetClickableIcon().Build();
		transmitterSelector.Initialize(_dropdownItemsSetter, _eventBus, _transmitterPickerTool, transmitterDropdownProvider, automationStateIcon, setter, closeIconClicked);
		_tooltipRegistrar.RegisterLocalizable(transmitterSelector.Q<Button>("CloseIcon"), RemoveRowLocKey);
	}
}
