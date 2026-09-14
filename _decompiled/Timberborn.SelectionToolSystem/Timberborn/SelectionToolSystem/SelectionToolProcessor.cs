using System;
using System.Collections.Generic;
using Timberborn.AreaSelectionSystem;
using Timberborn.InputSystem;
using UnityEngine;

namespace Timberborn.SelectionToolSystem;

public class SelectionToolProcessor : IInputProcessor
{
	private readonly AreaPicker _areaPicker;

	private readonly InputService _inputService;

	private readonly CursorService _cursorService;

	private readonly Action<IEnumerable<Vector3Int>, Ray> _previewCallback;

	private readonly Action<IEnumerable<Vector3Int>, Ray> _actionCallback;

	private readonly Action _showNoneCallback;

	private readonly string _customCursor;

	public SelectionToolProcessor(AreaPicker areaPicker, InputService inputService, CursorService cursorService, Action<IEnumerable<Vector3Int>, Ray> previewCallback, Action<IEnumerable<Vector3Int>, Ray> actionCallback, Action showNoneCallback, string customCursor)
	{
		_areaPicker = areaPicker;
		_inputService = inputService;
		_cursorService = cursorService;
		_previewCallback = previewCallback;
		_actionCallback = actionCallback;
		_showNoneCallback = showNoneCallback;
		_customCursor = customCursor;
	}

	public bool ProcessInput()
	{
		return _areaPicker.PickTerrainIntArea(delegate(IEnumerable<Vector3Int> blocks, Ray ray)
		{
			_previewCallback(blocks, ray);
		}, delegate(IEnumerable<Vector3Int> blocks, Ray ray)
		{
			_actionCallback(blocks, ray);
		}, _showNoneCallback);
	}

	public void Enter()
	{
		_inputService.AddInputProcessor(this);
		_cursorService.SetCursor(_customCursor);
	}

	public void Exit()
	{
		_inputService.RemoveInputProcessor(this);
		_areaPicker.Reset();
		_cursorService.ResetCursor();
		_showNoneCallback();
	}
}
