using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.KeyBindingSystem;
using Timberborn.LocalizationSerialization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.KeyBindingSystemUI;

public class KeyBindingRowFactory
{
	private static readonly string DisabledElementClass = "disabled";

	private readonly KeyBindingGroupSpecService _keyBindingGroupSpecService;

	private readonly KeyBindingRegistry _keyBindingRegistry;

	private readonly KeyBindingShortcutService _keyBindingShortcutService;

	private readonly KeyRebinder _keyRebinder;

	private readonly VisualElementLoader _visualElementLoader;

	public KeyBindingRowFactory(KeyBindingGroupSpecService keyBindingGroupSpecService, KeyBindingRegistry keyBindingRegistry, KeyBindingShortcutService keyBindingShortcutService, KeyRebinder keyRebinder, VisualElementLoader visualElementLoader)
	{
		_keyBindingGroupSpecService = keyBindingGroupSpecService;
		_keyBindingRegistry = keyBindingRegistry;
		_keyBindingShortcutService = keyBindingShortcutService;
		_keyRebinder = keyRebinder;
		_visualElementLoader = visualElementLoader;
	}

	public IEnumerable<KeyBindingGroup> CreateAll()
	{
		foreach (KeyBindingGroupSpec keyBindingGroupSpec in _keyBindingGroupSpecService.KeyBindingGroupSpecs)
		{
			yield return CreateGroup(keyBindingGroupSpec);
		}
	}

	private KeyBindingGroup CreateGroup(KeyBindingGroupSpec keyBindingGroupSpec)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Options/KeyBindingGroup");
		visualElement.Q<Label>("Header").text = GetDisplayName(keyBindingGroupSpec);
		VisualElement parent = visualElement.Q<VisualElement>("Items");
		foreach (KeyBinding keyBinding in _keyBindingRegistry.KeyBindings)
		{
			if (keyBinding.GroupId == keyBindingGroupSpec.Id)
			{
				CreateKeyBindingRow(keyBinding, parent);
			}
		}
		return new KeyBindingGroup(visualElement, keyBindingGroupSpec);
	}

	private void CreateKeyBindingRow(KeyBinding keyBinding, VisualElement parent)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Options/KeyBindingRow");
		visualElement.Q<Label>("Name").text = keyBinding.DisplayName;
		parent.Add(visualElement);
		CreateElement(visualElement, keyBinding, isPrimary: true);
		CreateElement(visualElement, keyBinding, isPrimary: false);
	}

	private void CreateElement(VisualElement root, KeyBinding keyBinding, bool isPrimary)
	{
		DefinableInputBinding definableInputBinding = new DefinableInputBinding(keyBinding, isPrimary);
		Button button = root.Q<Button>(isPrimary ? "PrimaryInput" : "SecondaryInput");
		if (definableInputBinding.GetSingleInputBinding().InputBindingSpec.Unchangeable)
		{
			button.AddToClassList(DisabledElementClass);
			button.SetEnabled(value: false);
		}
		else
		{
			button.RegisterCallback<ClickEvent>(delegate
			{
				_keyRebinder.StartRebinding(definableInputBinding);
			});
		}
		_keyBindingShortcutService.CreateSingle(button, definableInputBinding);
	}

	private static string GetDisplayName(KeyBindingGroupSpec keyBindingGroupSpec)
	{
		LocalizedText displayName = keyBindingGroupSpec.DisplayName;
		string id = keyBindingGroupSpec.Id;
		if (displayName == null)
		{
			if (!keyBindingGroupSpec.IsHiddenGroup)
			{
				Debug.LogWarning("Loc key not defined for key binding group: " + id);
			}
			return "<color=\"orange\">" + id + "</color>";
		}
		return displayName.Value;
	}
}
