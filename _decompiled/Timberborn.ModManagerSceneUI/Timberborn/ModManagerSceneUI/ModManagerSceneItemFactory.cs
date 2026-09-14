using System;
using Timberborn.Modding;
using Timberborn.ModdingUI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.ModManagerSceneUI;

public class ModManagerSceneItemFactory : MonoBehaviour, IModItemFactory
{
	[SerializeField]
	private VisualTreeAsset _modItemVisualTreeAsset;

	public ModItem CreateModItem(Mod mod, Action<Mod, bool> onPriorityIncreased, Action<Mod, bool> onPriorityDecreased)
	{
		VisualElement root = _modItemVisualTreeAsset.CloneTree().ElementAt(0);
		ModItem modItem = new ModItem(GetComponent<ModManagerSceneTooltipRegistrar>(), root, mod, IsShiftPressed);
		modItem.Initialize(onPriorityIncreased, onPriorityDecreased);
		return modItem;
	}

	private static bool IsShiftPressed()
	{
		if (!Keyboard.current[Key.LeftShift].isPressed)
		{
			return Keyboard.current[Key.RightShift].isPressed;
		}
		return true;
	}
}
