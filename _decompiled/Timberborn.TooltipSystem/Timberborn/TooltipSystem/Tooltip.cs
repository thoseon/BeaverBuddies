using System;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.TooltipSystem;

internal class Tooltip : ILoadableSingleton, IUpdatableSingleton
{
	private static readonly float HideTooltipMinDistanceSqr = 2f;

	private static readonly float NextTooltipTiming = 0.2f;

	private static readonly float TooltipVisibilityTime = 15f;

	private readonly TooltipBlocker _tooltipBlocker;

	private readonly TooltipContainer _tooltipContainer;

	private readonly VisualElementLoader _visualElementLoader;

	private TooltipContent _tooltipContent;

	private VisualElement _tooltipRoot;

	private VisualElement _keyBindingRoot;

	private Label _tooltipLabel;

	private Label _keyBindingLabel;

	private bool _wasVisibleLastUpdate;

	private bool _enabled;

	private float _showTimestamp;

	private float _hideTimestamp;

	public Tooltip(TooltipBlocker tooltipBlocker, TooltipContainer tooltipContainer, VisualElementLoader visualElementLoader)
	{
		_tooltipBlocker = tooltipBlocker;
		_tooltipContainer = tooltipContainer;
		_visualElementLoader = visualElementLoader;
	}

	public void Load()
	{
		_tooltipRoot = _visualElementLoader.LoadVisualElement("Core/Tooltip");
		_tooltipLabel = _tooltipRoot.Q<Label>("Description");
		_keyBindingLabel = _tooltipRoot.Q<Label>("KeyBinding");
		_keyBindingRoot = _tooltipRoot.Q<VisualElement>("KeyBindingRoot");
	}

	public void UpdateSingleton()
	{
		float unscaledTime = Time.unscaledTime;
		bool flag = _enabled && _tooltipBlocker.IsUnblocked && unscaledTime >= _showTimestamp && unscaledTime < _hideTimestamp && (!_tooltipContent.UpdatableContent || _tooltipContent.HasContent());
		if (!_wasVisibleLastUpdate & flag)
		{
			_tooltipContainer.Show(_tooltipRoot);
		}
		else if (_wasVisibleLastUpdate && !flag)
		{
			_tooltipContainer.Clear();
		}
		if (flag && _tooltipContent.UpdatableContent)
		{
			UpdateTooltipContent();
		}
		_wasVisibleLastUpdate = flag;
	}

	public void RegisterTooltip(VisualElement visualElement, Func<TooltipContent> tooltipContentGetter)
	{
		visualElement.RegisterCallback<MouseEnterEvent>(delegate
		{
			Enable(tooltipContentGetter());
		});
		visualElement.RegisterCallback<MouseLeaveEvent>(delegate
		{
			Disable();
		});
		visualElement.RegisterCallback<MouseMoveEvent>(OnPointerMove);
		visualElement.RegisterCallback<MouseUpEvent>(delegate
		{
			Disable();
		});
		visualElement.RegisterCallback<DetachFromPanelEvent>(delegate
		{
			Disable();
		});
	}

	private void Enable(TooltipContent tooltipContent)
	{
		if (tooltipContent.HasContent() || tooltipContent.UpdatableContent)
		{
			_tooltipContent = tooltipContent;
			UpdateTooltipContent();
			_enabled = true;
			if (!_wasVisibleLastUpdate)
			{
				UpdateShowTimestamp();
				UpdateHideTimestamp();
			}
		}
	}

	private void UpdateTooltipContent()
	{
		_tooltipLabel.Clear();
		_tooltipLabel.text = _tooltipContent.BaseText;
		_tooltipLabel.Add(_tooltipContent.VisualElement);
		if (_tooltipContent.TryGetKeyBinding(out var keyBinding))
		{
			_keyBindingLabel.text = keyBinding;
			_keyBindingRoot.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_keyBindingRoot.ToggleDisplayStyle(visible: false);
		}
		IgnorePicking(_tooltipLabel);
	}

	private static void IgnorePicking(VisualElement visualElement)
	{
		visualElement.pickingMode = PickingMode.Ignore;
		foreach (VisualElement item in visualElement.Children())
		{
			IgnorePicking(item);
		}
	}

	private void Disable()
	{
		_enabled = false;
		_hideTimestamp = Time.unscaledTime + NextTooltipTiming;
	}

	private void OnPointerMove(MouseMoveEvent mouseMoveEvent)
	{
		if (_enabled)
		{
			if (ShouldResetTooltip(mouseMoveEvent.mouseDelta.sqrMagnitude))
			{
				UpdateShowTimestamp();
			}
			UpdateHideTimestamp();
		}
	}

	private bool ShouldResetTooltip(float mouseDistanceSqr)
	{
		if (!(mouseDistanceSqr > HideTooltipMinDistanceSqr) || !(_tooltipContent.Delay > 0f))
		{
			return !_wasVisibleLastUpdate;
		}
		return true;
	}

	private void UpdateShowTimestamp()
	{
		_showTimestamp = Time.unscaledTime + _tooltipContent.Delay;
	}

	private void UpdateHideTimestamp()
	{
		_hideTimestamp = _showTimestamp + TooltipVisibilityTime;
	}
}
