using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InputSystemUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.DuplicationSystemUI;

internal class DuplicateSettingsFragment : IEntityPanelFragment
{
	private static readonly string DuplicateSettingsKey = "DuplicateSettings";

	private readonly DuplicationValidator _duplicationValidator;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly DuplicateSettingsTool _duplicateSettingsTool;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private BaseComponent _selectedDuplicable;

	private BindableButton _button;

	private VisualElement _root;

	public DuplicateSettingsFragment(DuplicationValidator duplicationValidator, VisualElementLoader visualElementLoader, BindableButtonFactory bindableButtonFactory, DuplicateSettingsTool duplicateSettingsTool, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_duplicationValidator = duplicationValidator;
		_visualElementLoader = visualElementLoader;
		_bindableButtonFactory = bindableButtonFactory;
		_duplicateSettingsTool = duplicateSettingsTool;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/EntityPanel/DuplicateSettingsFragment");
		Button button = _root.Q<Button>("Button");
		_button = _bindableButtonFactory.Create(button, DuplicateSettingsKey, Callback);
		_tooltipRegistrar.RegisterWithKeyBinding(button, DuplicateSettingsKey);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (_duplicationValidator.CanDuplicateSettings(entity))
		{
			_selectedDuplicable = entity;
			_root.ToggleDisplayStyle(visible: true);
			_button.Bind();
		}
	}

	public void ClearFragment()
	{
		_selectedDuplicable = null;
		_button.Unbind();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
	}

	private void Callback()
	{
		_duplicateSettingsTool.ActivateWithSource(_selectedDuplicable);
	}
}
