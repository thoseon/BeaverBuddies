using System;
using System.Collections.Immutable;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.DropdownSystem;

public class DropdownItemsSetter
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	public DropdownItemsSetter(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public void SetItems(Dropdown dropdown, IDropdownProvider dropdownProvider)
	{
		SetItems(dropdown, dropdownProvider, Create);
	}

	public void SetItems(Dropdown dropdown, IExtendedDropdownProvider dropdownProvider)
	{
		SetItems(dropdown, dropdownProvider, (string value, bool selected) => Create(dropdownProvider.FormatDisplayText(value, selected), dropdownProvider.GetIcon(value), dropdownProvider.GetItemClasses(value)));
	}

	public void SetItems(Dropdown dropdown, IExtendedTooltipDropdownProvider dropdownProvider)
	{
		SetItems(dropdown, dropdownProvider, (string value, bool selected) => Create(dropdownProvider.FormatDisplayText(value, selected), dropdownProvider.GetIcon(value), dropdownProvider.GetItemClasses(value), dropdownProvider.GetDropdownTooltip(value)));
	}

	public void SetLocalizableItems(Dropdown dropdown, IExtendedDropdownProvider dropdownProvider)
	{
		SetItems(dropdown, dropdownProvider, (string value, bool selected) => CreateLocalizable(dropdownProvider.FormatDisplayText(value, selected), dropdownProvider.GetIcon(value), dropdownProvider.GetItemClasses(value)));
	}

	private void SetItems(Dropdown dropdown, IDropdownProvider dropdownProvider, Func<string, bool, DropdownElement> visualElementGetter)
	{
		dropdown.ClearItems();
		dropdown.SetItems(dropdownProvider, _tooltipRegistrar, visualElementGetter);
	}

	private DropdownElement CreateLocalizable(string value, Sprite icon, ImmutableArray<string> itemClasses)
	{
		return Create(_loc.T(value), icon, itemClasses);
	}

	private DropdownElement Create(string text, Sprite icon, ImmutableArray<string> itemClasses, string tooltip = null)
	{
		VisualElement visualElement = CreateContent(text);
		foreach (string item in itemClasses)
		{
			visualElement.AddToClassList(item);
		}
		Image image = visualElement.Q<Image>("Icon");
		image.sprite = icon;
		image.ToggleDisplayStyle(icon);
		return new DropdownElement(visualElement, tooltip);
	}

	private DropdownElement Create(string text, bool selected)
	{
		return new DropdownElement(CreateContent(text), null);
	}

	private VisualElement CreateContent(string text)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Core/DropdownItem");
		visualElement.Q<Label>("Text").text = text;
		return visualElement;
	}
}
