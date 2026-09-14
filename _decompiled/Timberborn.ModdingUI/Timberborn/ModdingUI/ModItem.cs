using System;
using Timberborn.Modding;
using UnityEngine.UIElements;

namespace Timberborn.ModdingUI;

public class ModItem
{
	private static readonly string LocalModIconClass = "mod-item__icon--local";

	private static readonly string CloudModIconClass = "mod-item__icon--cloud";

	private static readonly string PriorityModifierClass = "mod-item__priority-modifier";

	private readonly IModManagerTooltipRegistrar _modManagerTooltipRegistrar;

	private readonly Func<bool> _priorityChangeModifier;

	private VisualElement _warningIcon;

	public VisualElement Root { get; }

	public Mod Mod { get; }

	public ModWarningReason WarningReason { get; private set; }

	public string WarningInfo { get; private set; }

	public ModManifest ModManifest => Mod.Manifest;

	public event EventHandler ModToggled;

	public ModItem(IModManagerTooltipRegistrar modManagerTooltipRegistrar, VisualElement root, Mod mod, Func<bool> priorityChangeModifier)
	{
		_modManagerTooltipRegistrar = modManagerTooltipRegistrar;
		Root = root;
		Mod = mod;
		_priorityChangeModifier = priorityChangeModifier;
	}

	public void Update()
	{
		Root.EnableInClassList(PriorityModifierClass, _priorityChangeModifier());
	}

	public void Initialize(Action<Mod, bool> onPriorityIncreased, Action<Mod, bool> onPriorityDecreased)
	{
		_warningIcon = Root.Q<VisualElement>("WarningIcon");
		Toggle toggle = Root.Q<Toggle>("ModToggle");
		toggle.value = ModPlayerPrefsHelper.IsModEnabled(Mod);
		toggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
		{
			ToggleMod(evt.newValue);
		});
		VisualElement visualElement = Root.Q<VisualElement>("ModIcon");
		visualElement.AddToClassList(Mod.ModDirectory.IsUserMod ? LocalModIconClass : CloudModIconClass);
		_modManagerTooltipRegistrar.RegisterModIcon(visualElement, this);
		Label label = Root.Q<Label>("ModName");
		label.enableRichText = false;
		label.text = Mod.DisplayName;
		Root.Q<Label>("ModVersion").text = ModManifest.Version.Formatted;
		_modManagerTooltipRegistrar.RegisterModWarning(_warningIcon, this);
		Button button = Root.Q<Button>("Increase");
		button.RegisterCallback<ClickEvent>(delegate
		{
			onPriorityIncreased(Mod, _priorityChangeModifier());
		});
		_modManagerTooltipRegistrar.RegisterIncreaseButton(button);
		Button button2 = Root.Q<Button>("Decrease");
		button2.RegisterCallback<ClickEvent>(delegate
		{
			onPriorityDecreased(Mod, _priorityChangeModifier());
		});
		_modManagerTooltipRegistrar.RegisterDecreaseButton(button2);
	}

	public void SetWarning(ModWarningReason warningReason, string warningInfo)
	{
		WarningReason = warningReason;
		WarningInfo = warningInfo;
		_warningIcon.style.display = ((WarningReason == ModWarningReason.None) ? DisplayStyle.None : DisplayStyle.Flex);
	}

	private void ToggleMod(bool isEnabled)
	{
		ModPlayerPrefsHelper.ToggleMod(isEnabled, Mod);
		ModToggled?.Invoke(this, EventArgs.Empty);
	}
}
