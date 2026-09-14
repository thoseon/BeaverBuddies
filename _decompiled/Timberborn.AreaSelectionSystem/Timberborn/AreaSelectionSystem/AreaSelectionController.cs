using System;
using Timberborn.CameraSystem;
using Timberborn.InputSystem;
using UnityEngine;

namespace Timberborn.AreaSelectionSystem;

public class AreaSelectionController
{
	public delegate void RaysCallback(Ray start, Ray end, bool selectionStarted);

	private readonly InputService _inputService;

	private readonly CameraService _cameraService;

	private bool _camRotationStartedFirst;

	private bool _camRotationStarted;

	private bool _selectionStarted;

	private Ray? _startRay;

	private Ray? _endRay;

	public AreaSelectionController(InputService inputService, CameraService cameraService)
	{
		_inputService = inputService;
		_cameraService = cameraService;
	}

	public bool ProcessInput(RaysCallback previewCallback, RaysCallback actionCallback, Action showNoneCallback)
	{
		if (_inputService.MainMouseButtonUp && !_camRotationStartedFirst)
		{
			if (_startRay.HasValue)
			{
				actionCallback(_startRay.Value, _endRay ?? _startRay.Value, _selectionStarted);
				_camRotationStarted = false;
			}
			_selectionStarted = false;
			_startRay = null;
			_endRay = null;
			return true;
		}
		if (_inputService.RotateButtonUp)
		{
			_camRotationStartedFirst = false;
			_camRotationStarted = false;
		}
		Ray value = _cameraService.ScreenPointToRayInGridSpace(_inputService.MousePosition);
		bool result = false;
		if (!_selectionStarted && _inputService.RotateButtonHeld)
		{
			if (!_camRotationStartedFirst)
			{
				_camRotationStartedFirst = true;
				_startRay = null;
				_endRay = null;
			}
		}
		else if (!_camRotationStartedFirst)
		{
			if (_inputService.MainMouseButtonDown)
			{
				_selectionStarted = true;
				_endRay = null;
				_startRay = (_inputService.MouseOverUI ? ((Ray?)null) : new Ray?(value));
			}
			else if (_selectionStarted && _inputService.Cancel)
			{
				if (!_startRay.HasValue && !_endRay.HasValue)
				{
					return false;
				}
				_startRay = null;
				_endRay = null;
				result = true;
			}
			else if (_inputService.MainMouseButtonHeld && _inputService.RotateButtonDown)
			{
				if (!_camRotationStarted)
				{
					_camRotationStarted = true;
					if (_startRay.HasValue)
					{
						_endRay = value;
					}
				}
			}
			else if (_inputService.MainMouseButtonHeld && !_camRotationStarted)
			{
				if (_startRay.HasValue)
				{
					_endRay = value;
				}
			}
			else if (!_camRotationStarted && !_selectionStarted)
			{
				_startRay = (_inputService.MouseOverUI ? ((Ray?)null) : new Ray?(value));
				_endRay = null;
			}
		}
		if (_startRay.HasValue)
		{
			previewCallback(_startRay.Value, _endRay ?? _startRay.Value, _selectionStarted);
		}
		else
		{
			showNoneCallback();
		}
		return result;
	}

	public void Reset()
	{
		_startRay = null;
		_endRay = null;
		_selectionStarted = false;
		_camRotationStartedFirst = false;
		_camRotationStarted = false;
	}
}
