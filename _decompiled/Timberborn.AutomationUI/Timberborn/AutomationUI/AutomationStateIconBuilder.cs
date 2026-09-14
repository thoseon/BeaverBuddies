using System;
using Timberborn.Automation;
using Timberborn.SelectionSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationUI;

public class AutomationStateIconBuilder
{
	public class Builder
	{
		private readonly EntitySelectionService _entitySelectionService;

		private readonly Image _icon;

		private readonly Func<Automator> _automatorGetter;

		private bool _clickable;

		public Builder(EntitySelectionService entitySelectionService, Image icon, Func<Automator> automatorGetter)
		{
			_entitySelectionService = entitySelectionService;
			_icon = icon;
			_automatorGetter = automatorGetter;
		}

		public Builder SetClickableIcon()
		{
			_clickable = true;
			return this;
		}

		public AutomationStateIcon Build()
		{
			AutomationStateIcon result = new AutomationStateIcon(_automatorGetter, _icon);
			if (_clickable)
			{
				_icon.RegisterCallback<ClickEvent>(delegate
				{
					OnStateIconClicked(_automatorGetter);
				});
				_icon.AddToClassList(ClickableClass);
			}
			return result;
		}

		private void OnStateIconClicked(Func<Automator> automatorGetter)
		{
			Automator automator = automatorGetter();
			if (automator != null)
			{
				_entitySelectionService.SelectAndFocusOn(automator);
			}
		}
	}

	private static readonly string ClickableClass = "clickable";

	private readonly EntitySelectionService _entitySelectionService;

	public AutomationStateIconBuilder(EntitySelectionService entitySelectionService)
	{
		_entitySelectionService = entitySelectionService;
	}

	public Builder Create(Image icon, Func<Automator> automatorGetter)
	{
		return new Builder(_entitySelectionService, icon, automatorGetter);
	}
}
