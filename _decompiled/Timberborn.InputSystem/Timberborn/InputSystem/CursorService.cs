using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.InputSystem;

public class CursorService : ILoadableSingleton
{
	private static readonly string DefaultCursorName = "DefaultCursor";

	private readonly ISpecService _specService;

	private CustomCursorSpec _cursorSpec;

	private bool _useMacOsCursor;

	private Dictionary<string, CustomCursorSpec> _cursorSpecs = new Dictionary<string, CustomCursorSpec>();

	public Vector2 CursorOffset
	{
		get
		{
			if (!_useMacOsCursor)
			{
				return _cursorSpec.WindowsCursorOffset;
			}
			return _cursorSpec.MacOsCursorOffset;
		}
	}

	public CursorService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_useMacOsCursor = ApplicationPlatform.IsMacOS();
		_cursorSpecs = (from spec in _specService.GetSpecs<CustomCursorSpec>()
			where spec.Blueprint.IsAllowedByFeatureToggles()
			select spec).ToDictionary((CustomCursorSpec cursor) => cursor.Id);
		ResetCursor();
	}

	public void SetCursor(string cursorName)
	{
		SetCursor(_cursorSpecs[cursorName]);
	}

	public void SetTemporaryCursor(string cursorName)
	{
		SetCursorImage(_cursorSpecs[cursorName]);
	}

	public void ResetCursor()
	{
		SetCursor(DefaultCursorName);
	}

	public void ResetTemporaryCursor()
	{
		SetCursor(_cursorSpec);
	}

	private void SetCursor(CustomCursorSpec cursorSpec)
	{
		SetCursorImage(cursorSpec);
		_cursorSpec = cursorSpec;
	}

	private void SetCursorImage(CustomCursorSpec cursorSpec)
	{
		Cursor.SetCursor((_useMacOsCursor ? cursorSpec.MacOsCursor : cursorSpec.WindowsCursor)?.Asset, cursorSpec.Hotspot, CursorMode.Auto);
	}
}
