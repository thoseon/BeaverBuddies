using Timberborn.ConstructionMode;
using Timberborn.CursorToolSystem;
using Timberborn.InputSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.LevelVisibilitySystemUI;

internal class LevelVisibilityPicker : IInputProcessor, ILoadableSingleton
{
	private static readonly string PickVisibleLayerKey = "PickVisibleLayer";

	private readonly InputService _inputService;

	private readonly CursorCoordinatesPicker _cursorCoordinatesPicker;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly ConstructionModeService _constructionModeService;

	public LevelVisibilityPicker(InputService inputService, CursorCoordinatesPicker cursorCoordinatesPicker, ILevelVisibilityService levelVisibilityService, ConstructionModeService constructionModeService)
	{
		_inputService = inputService;
		_cursorCoordinatesPicker = cursorCoordinatesPicker;
		_levelVisibilityService = levelVisibilityService;
		_constructionModeService = constructionModeService;
	}

	public void Load()
	{
		_inputService.AddInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(PickVisibleLayerKey))
		{
			PickOrResetLayer();
			return true;
		}
		return false;
	}

	private void PickOrResetLayer()
	{
		CursorCoordinates? cursorCoordinates = (_constructionModeService.InConstructionMode ? _cursorCoordinatesPicker.Pick() : _cursorCoordinatesPicker.PickOnFinished());
		if (cursorCoordinates.HasValue)
		{
			int z = cursorCoordinates.GetValueOrDefault().TileCoordinates.z;
			if (z < _levelVisibilityService.MaxVisibleLevel)
			{
				_levelVisibilityService.SetMaxVisibleLevel(z);
			}
			else
			{
				_levelVisibilityService.ResetMaxVisibleLevel();
			}
		}
	}
}
