using System;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.Modding;
using Timberborn.ModdingUI;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuModdingUI;

internal class MainMenuModItemFactory : IModItemFactory
{
	private static readonly string AlternateClickableActionKey = "AlternateClickableAction";

	private readonly IModManagerTooltipRegistrar _modManagerTooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly InputService _inputService;

	public MainMenuModItemFactory(IModManagerTooltipRegistrar modManagerTooltipRegistrar, VisualElementLoader visualElementLoader, InputService inputService)
	{
		_modManagerTooltipRegistrar = modManagerTooltipRegistrar;
		_visualElementLoader = visualElementLoader;
		_inputService = inputService;
	}

	public ModItem CreateModItem(Mod mod, Action<Mod, bool> onPriorityIncreased, Action<Mod, bool> onPriorityDecreased)
	{
		VisualElement root = _visualElementLoader.LoadVisualElement("Modding/ModItem");
		ModItem modItem = new ModItem(_modManagerTooltipRegistrar, root, mod, IsAlternateKeyHeld);
		modItem.Initialize(onPriorityIncreased, onPriorityDecreased);
		return modItem;
	}

	private bool IsAlternateKeyHeld()
	{
		return _inputService.IsKeyHeld(AlternateClickableActionKey);
	}
}
