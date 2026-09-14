using System;
using System.Collections.Generic;
using Timberborn.AreaSelectionSystem;
using Timberborn.InputSystem;
using UnityEngine;

namespace Timberborn.SelectionToolSystem;

public class SelectionToolProcessorFactory
{
	private readonly AreaPicker _areaPicker;

	private readonly InputService _inputService;

	private readonly CursorService _cursorService;

	public SelectionToolProcessorFactory(AreaPicker areaPicker, InputService inputService, CursorService cursorService)
	{
		_areaPicker = areaPicker;
		_inputService = inputService;
		_cursorService = cursorService;
	}

	public SelectionToolProcessor Create(Action<IEnumerable<Vector3Int>, Ray> previewCallback, Action<IEnumerable<Vector3Int>, Ray> actionCallback, Action showNoneCallback, string customCursor)
	{
		return new SelectionToolProcessor(_areaPicker, _inputService, _cursorService, previewCallback, actionCallback, showNoneCallback, customCursor);
	}
}
