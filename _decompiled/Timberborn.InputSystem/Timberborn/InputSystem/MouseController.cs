using Timberborn.PlatformUtilities;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Timberborn.InputSystem;

public class MouseController : ILoadableSingleton, IPostLoadableSingleton, ILateUpdatableSingleton
{
	private static readonly int MovementFramesToIgnoreAfterLockOnMacOS = 3;

	private static readonly float MacOsMouseDeltaMultiplier = 7.5f;

	private readonly InputSettings _inputSettings;

	private int _ignoredMovementFrames;

	private bool _cursorIsForceHidden;

	private Vector2? _lockedPosition;

	public Vector3 Position => Mouse.current.position.ReadValue();

	public Vector2 XYAxes
	{
		get
		{
			if (_ignoredMovementFrames <= 0)
			{
				return XYAxesInternal;
			}
			return default(Vector2);
		}
	}

	public bool IsCursorVisible => Cursor.visible;

	private static Vector2 XYAxesInternal => Mouse.current.delta.ReadValue() * (ApplicationPlatform.IsMacOS() ? MacOsMouseDeltaMultiplier : 1f);

	public MouseController(InputSettings inputSettings)
	{
		_inputSettings = inputSettings;
	}

	public void Load()
	{
		_inputSettings.LockCursorInWindowChanged += OnLockCursorInWindowChanged;
	}

	public void PostLoad()
	{
		UpdateCursorLockState();
	}

	public void LateUpdateSingleton()
	{
		if (_ignoredMovementFrames > 0 && XYAxesInternal != default(Vector2))
		{
			_ignoredMovementFrames--;
		}
		if (_lockedPosition.HasValue)
		{
			Mouse.current.WarpCursorPosition(_lockedPosition.Value);
		}
	}

	public void HideCursor()
	{
		Cursor.visible = false;
	}

	public void ShowCursor()
	{
		if (!_cursorIsForceHidden)
		{
			Cursor.visible = true;
		}
	}

	public void ForceHideCursor()
	{
		HideCursor();
		_cursorIsForceHidden = true;
	}

	public void ForceShowCursor()
	{
		_cursorIsForceHidden = false;
		ShowCursor();
	}

	public void LockCursor()
	{
		_lockedPosition = Position;
		Cursor.lockState = CursorLockMode.Confined;
		_ignoredMovementFrames = (ApplicationPlatform.IsMacOS() ? MovementFramesToIgnoreAfterLockOnMacOS : 0);
	}

	public void UnlockCursor()
	{
		_lockedPosition = null;
		UpdateCursorLockState();
		_ignoredMovementFrames = 0;
	}

	private void OnLockCursorInWindowChanged(object sender, SettingChangedEventArgs<bool> e)
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			UpdateCursorLockState();
		}
	}

	private void UpdateCursorLockState()
	{
		Cursor.lockState = (_inputSettings.LockCursorInWindow ? CursorLockMode.Confined : CursorLockMode.None);
	}
}
