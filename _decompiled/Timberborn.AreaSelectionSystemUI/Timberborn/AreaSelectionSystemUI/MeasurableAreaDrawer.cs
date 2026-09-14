using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.ToolPanelSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.AreaSelectionSystemUI;

public class MeasurableAreaDrawer : ILateUpdatableSingleton, IToolFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly List<Vector3Int> _areaCoordinates = new List<Vector3Int>();

	private VisualElement _root;

	private Label _dimensions;

	private bool _isDrawing;

	private bool _frameDelayed;

	public MeasurableAreaDrawer(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/MeasurableAreaPanel");
		_dimensions = _root.Q<Label>("Dimensions");
		return _root;
	}

	public void LateUpdateSingleton()
	{
		UpdateDrawingState();
		UpdatePanel();
		_areaCoordinates.Clear();
	}

	public void AddMeasurableCoordinates(Vector3Int coordinates)
	{
		_areaCoordinates.Add(coordinates);
	}

	public void AddMeasurableCoordinates(IEnumerable<Vector3Int> coordinates)
	{
		_areaCoordinates.AddRange(coordinates);
	}

	private void UpdateDrawingState()
	{
		if (!_isDrawing && _areaCoordinates.Count > 1)
		{
			if (_frameDelayed)
			{
				_isDrawing = true;
			}
			_frameDelayed = true;
		}
		else if (_isDrawing && _areaCoordinates.Count <= 1)
		{
			_frameDelayed = false;
			_isDrawing = false;
		}
	}

	private void UpdatePanel()
	{
		if (_isDrawing)
		{
			_root.ToggleDisplayStyle(visible: true);
			UpdateDimensions();
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void UpdateDimensions()
	{
		int num = int.MaxValue;
		int num2 = int.MinValue;
		int num3 = int.MaxValue;
		int num4 = int.MinValue;
		foreach (Vector3Int areaCoordinate in _areaCoordinates)
		{
			num = Mathf.Min(num, areaCoordinate.x);
			num2 = Mathf.Max(num2, areaCoordinate.x);
			num3 = Mathf.Min(num3, areaCoordinate.y);
			num4 = Mathf.Max(num4, areaCoordinate.y);
		}
		_dimensions.text = $"{num2 - num + 1} × {num4 - num3 + 1}";
	}
}
