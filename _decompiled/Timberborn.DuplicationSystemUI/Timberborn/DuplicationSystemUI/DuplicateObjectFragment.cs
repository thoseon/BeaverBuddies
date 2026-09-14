using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InputSystemUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.DuplicationSystemUI;

internal class DuplicateObjectFragment : IEntityPanelFragment
{
	private static readonly string DuplicateObjectKey = "DuplicateObject";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly DuplicationValidator _duplicationValidator;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private BindableButton _button;

	private VisualElement _root;

	private Action _toolActivationAction;

	public DuplicateObjectFragment(VisualElementLoader visualElementLoader, BindableButtonFactory bindableButtonFactory, DuplicationValidator duplicationValidator, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_bindableButtonFactory = bindableButtonFactory;
		_duplicationValidator = duplicationValidator;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/EntityPanel/DuplicateObjectFragment");
		Button button = _root.Q<Button>("Button");
		_button = _bindableButtonFactory.Create(button, DuplicateObjectKey, Callback);
		_tooltipRegistrar.RegisterWithKeyBinding(button, DuplicateObjectKey);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (_duplicationValidator.CanDuplicateObject(entity, out var toolActivationAction))
		{
			_toolActivationAction = toolActivationAction;
			_root.ToggleDisplayStyle(visible: true);
			_button.Bind();
		}
	}

	public void ClearFragment()
	{
		_toolActivationAction = null;
		_button.Unbind();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
	}

	private void Callback()
	{
		_toolActivationAction?.Invoke();
	}
}
