using System;
using Timberborn.InputSystem;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UIElements;

namespace Timberborn.TooltipSystem;

internal class MouseTooltipPositioner
{
	private static readonly int EditorTooltipOffset = 10;

	private readonly CursorService _cursorService;

	private readonly InputService _inputService;

	public MouseTooltipPositioner(CursorService cursorService, InputService inputService)
	{
		_cursorService = cursorService;
		_inputService = inputService;
	}

	public void UpdatePosition(VisualElement visualElement)
	{
		Vector2 mousePositionNdc = _inputService.MousePositionNdc;
		Vector2 vector = CalculateCursorOffset();
		visualElement.style.left = CalculateHorizontalPosition(visualElement, mousePositionNdc.x, vector.x);
		visualElement.style.top = CalculateVerticalPosition(visualElement, mousePositionNdc.y, vector.y);
	}

	private Vector2 CalculateCursorOffset()
	{
		Resolution currentResolution = UnityEngine.Device.Screen.currentResolution;
		float num = (float)UnityEngine.Screen.width / (float)currentResolution.width;
		float num2 = (float)UnityEngine.Screen.height / (float)currentResolution.height;
		Vector2 cursorOffset = _cursorService.CursorOffset;
		cursorOffset.y += (UnityEngine.Application.isEditor ? EditorTooltipOffset : 0);
		cursorOffset.x /= num;
		cursorOffset.y /= num2;
		return cursorOffset;
	}

	private static float CalculateHorizontalPosition(VisualElement visualElement, float mousePosition, float horizontalOffset)
	{
		float width = visualElement.parent.resolvedStyle.width;
		float width2 = visualElement.resolvedStyle.width;
		float num = mousePosition * width + horizontalOffset;
		if (!(num + width2 + horizontalOffset > width))
		{
			return num;
		}
		return width - width2;
	}

	private static float CalculateVerticalPosition(VisualElement visualElement, float mousePosition, float verticalOffset)
	{
		float height = visualElement.parent.resolvedStyle.height;
		float height2 = visualElement.resolvedStyle.height;
		return Math.Min((1f - mousePosition) * height + verticalOffset, height - height2);
	}
}
