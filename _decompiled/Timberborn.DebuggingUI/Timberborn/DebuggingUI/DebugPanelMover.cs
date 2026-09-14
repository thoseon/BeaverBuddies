using System;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.SettingsSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

public class DebugPanelMover
{
	private static readonly string SettingsKey = "DebugPanelMover.{0}.{1}";

	private static readonly string HasSavedPositionKey = "HasSavedPosition";

	private static readonly float PanelWidth = 500f;

	private static readonly int SafetyMargin = 20;

	private readonly InputService _inputService;

	private readonly ISettings _settings;

	private string _id;

	private VisualElement _root;

	private VisualElement _headerElement;

	private VisualElement _contentContainer;

	private Vector2? _mouseOffset;

	private bool IsVisible => _contentContainer.resolvedStyle.display != DisplayStyle.None;

	public DebugPanelMover(InputService inputService, ISettings settings)
	{
		_inputService = inputService;
		_settings = settings;
	}

	public void Initialize(string id, VisualElement root, VisualElement contentContainer)
	{
		Asserts.FieldIsNull(this, _root, "_root");
		_id = id;
		_root = root;
		_contentContainer = contentContainer;
		_headerElement = _root.Q<VisualElement>("PanelHeader");
		_headerElement.RegisterCallback<MouseDownEvent>(OnMouseDown);
		_headerElement.RegisterCallback<MouseMoveEvent>(OnMouseMove);
		_headerElement.RegisterCallback<MouseUpEvent>(OnMouseUp);
		_root.Q<Button>("MinimizeIcon").RegisterCallback<ClickEvent>(OnClick);
		if (TryLoadPanelPosition(out var position, out var visible))
		{
			SetPanelPosition(position);
			_contentContainer.ToggleDisplayStyle(visible);
		}
	}

	public void ResetPanelPosition()
	{
		_settings.Clear(GetKey("x"));
		_settings.Clear(GetKey("y"));
		_settings.Clear(GetKey("visible"));
		_settings.Clear(GetKey(HasSavedPositionKey));
		_root.style.left = StyleKeyword.Null;
		_root.style.top = StyleKeyword.Null;
		_contentContainer.ToggleDisplayStyle(visible: true);
	}

	private void OnMouseDown(MouseDownEvent evt)
	{
		if (evt.button == 0)
		{
			float x = _inputService.MousePositionNdc.x * _root.parent.resolvedStyle.width - _root.resolvedStyle.left;
			float y = (1f - _inputService.MousePositionNdc.y) * _root.parent.resolvedStyle.height - _root.resolvedStyle.top;
			_mouseOffset = new Vector2(x, y);
			_headerElement.CaptureMouse();
		}
	}

	private void OnMouseMove(MouseMoveEvent _)
	{
		if (_mouseOffset.HasValue)
		{
			float x = _inputService.MousePositionNdc.x * _root.parent.resolvedStyle.width - _mouseOffset.Value.x;
			float y = (1f - _inputService.MousePositionNdc.y) * _root.parent.resolvedStyle.height - _mouseOffset.Value.y;
			SetPanelPosition(new Vector2(x, y));
		}
	}

	private void OnMouseUp(MouseUpEvent evt)
	{
		if (evt.button == 0)
		{
			_mouseOffset = null;
			SavePanelPosition();
			_headerElement.ReleaseMouse();
		}
	}

	private void OnClick(ClickEvent evt)
	{
		_contentContainer.ToggleDisplayStyle(!IsVisible);
		SavePanelPosition();
	}

	private void SetPanelPosition(Vector2 position)
	{
		float width = _root.parent.resolvedStyle.width;
		float height = _root.parent.resolvedStyle.height;
		_root.style.left = Math.Clamp(position.x, 0f - PanelWidth + (float)SafetyMargin, width - (float)SafetyMargin);
		_root.style.top = Math.Clamp(position.y, -SafetyMargin, height - (float)SafetyMargin);
	}

	private void SavePanelPosition()
	{
		_settings.SetFloat(GetKey("x"), _root.style.left.value.value);
		_settings.SetFloat(GetKey("y"), _root.style.top.value.value);
		_settings.SetBool(GetKey("visible"), IsVisible);
		_settings.SetBool(GetKey(HasSavedPositionKey), value: true);
	}

	private bool TryLoadPanelPosition(out Vector2 position, out bool visible)
	{
		float safeFloat = _settings.GetSafeFloat(GetKey("x"), 0f);
		float safeFloat2 = _settings.GetSafeFloat(GetKey("y"), 0f);
		position = new Vector2(safeFloat, safeFloat2);
		visible = _settings.GetSafeBool(GetKey("visible"), defaultValue: true);
		return _settings.GetSafeBool(GetKey(HasSavedPositionKey));
	}

	private string GetKey(string keyId)
	{
		return string.Format(SettingsKey, _id, keyId);
	}
}
