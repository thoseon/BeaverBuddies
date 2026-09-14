using System;
using Timberborn.ModdingUI;
using Timberborn.PlatformUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.ModManagerSceneUI;

internal class ModManagerSceneTooltipRegistrar : MonoBehaviour, IModManagerTooltipRegistrar
{
	private VisualElement _tooltip;

	private Label _tooltipLabel;

	private float? _showTime;

	private Vector2 _cursorOffset;

	public void Awake()
	{
		UIDocument component = GetComponent<UIDocument>();
		_tooltip = component.rootVisualElement.Q<VisualElement>("ModManagerTooltip");
		_tooltipLabel = _tooltip.Q<Label>("TooltipLabel");
		ToggleTooltip(show: false);
		_cursorOffset = (ApplicationPlatform.IsMacOS() ? new Vector2(12f, 18f) : new Vector2(10f, 10f));
	}

	public void RegisterModWarning(VisualElement element, ModItem modItem)
	{
		element.RegisterCallback<MouseEnterEvent>(delegate
		{
			Show(GetWarningText(modItem));
		});
		element.RegisterCallback<MouseLeaveEvent>(delegate
		{
			Hide();
		});
	}

	public void RegisterModIcon(VisualElement element, ModItem modItem)
	{
		element.RegisterCallback<MouseEnterEvent>(delegate
		{
			Show(GetModSourceText(modItem));
		});
		element.RegisterCallback<MouseLeaveEvent>(delegate
		{
			Hide();
		});
	}

	public void RegisterIncreaseButton(VisualElement element)
	{
		element.RegisterCallback<MouseEnterEvent>(delegate
		{
			Show("Increase loading priority");
		});
		element.RegisterCallback<MouseLeaveEvent>(delegate
		{
			Hide();
		});
	}

	public void RegisterDecreaseButton(VisualElement element)
	{
		element.RegisterCallback<MouseEnterEvent>(delegate
		{
			Show("Decrease loading priority");
		});
		element.RegisterCallback<MouseLeaveEvent>(delegate
		{
			Hide();
		});
	}

	public void Update()
	{
		if (Time.unscaledTime > _showTime)
		{
			ToggleTooltip(show: true);
			UpdatePosition();
		}
	}

	private void ToggleTooltip(bool show)
	{
		_tooltip.style.display = ((!show) ? DisplayStyle.None : DisplayStyle.Flex);
	}

	private static string GetWarningText(ModItem modItem)
	{
		return modItem.WarningReason switch
		{
			ModWarningReason.MissingRequiredMod => "Missing required mod: \"" + modItem.WarningInfo + "\".", 
			ModWarningReason.RequiredModNotEnabled => "Required mod \"" + modItem.WarningInfo + "\" is not enabled.", 
			ModWarningReason.RequiredModInvalidVersion => "Required mod \"" + modItem.WarningInfo + "\" version is below the minimum required version.", 
			ModWarningReason.RequiredModInvalidOrder => "Required mod \"" + modItem.WarningInfo + "\" is below this mod in the load order.", 
			ModWarningReason.InvalidGameVersion => "Mod requires higher game version: " + modItem.WarningInfo + ".", 
			ModWarningReason.None => throw new ArgumentException("GetWarningText called with None warning reason"), 
			_ => throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}", "ModWarningReason", modItem.WarningReason)), 
		};
	}

	private static string GetModSourceText(ModItem modItem)
	{
		return modItem.Mod.ModDirectory.DisplaySource + " mod";
	}

	private void Show(string text)
	{
		_tooltipLabel.text = text;
		_showTime = Time.unscaledTime + 0.3f;
	}

	private void Hide()
	{
		ToggleTooltip(show: false);
		_showTime = null;
	}

	private void UpdatePosition()
	{
		Vector2 vector = Mouse.current.position.ReadValue();
		Vector2 vector2 = new Vector2(vector.x / (float)Screen.width, vector.y / (float)Screen.height);
		_tooltip.style.left = CalculateHorizontalPosition(vector2.x);
		float height = _tooltip.parent.resolvedStyle.height;
		_tooltip.style.top = (1f - vector2.y) * height + _cursorOffset.y;
	}

	private float CalculateHorizontalPosition(float mousePosition)
	{
		float width = _tooltip.parent.resolvedStyle.width;
		float width2 = _tooltip.resolvedStyle.width;
		float num = mousePosition * width + _cursorOffset.x;
		if (!(num + width2 + _cursorOffset.x > width))
		{
			return num;
		}
		return width - width2;
	}
}
