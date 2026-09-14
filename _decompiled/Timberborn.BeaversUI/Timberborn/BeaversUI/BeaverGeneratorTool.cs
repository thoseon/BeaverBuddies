using Timberborn.Beavers;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.CursorToolSystem;
using Timberborn.InputSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace Timberborn.BeaversUI;

internal class BeaverGeneratorTool : IDevModeTool, ITool, IInputProcessor
{
	private static readonly string CursorKey = "BeaverAvatarCursor";

	private static readonly string SpawnManyCharactersKey = "SpawnManyCharacters";

	private static readonly string SpawnChildKey = "SpawnChild";

	private static readonly int ManyBeaversToAdd = 10;

	private readonly BeaverFactory _beaverFactory;

	private readonly InputService _inputService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly CursorService _cursorService;

	private readonly CursorCoordinatesPicker _cursorCoordinatesPicker;

	public bool IsDevMode => true;

	public BeaverGeneratorTool(BeaverFactory beaverFactory, InputService inputService, IRandomNumberGenerator randomNumberGenerator, CursorService cursorService, CursorCoordinatesPicker cursorCoordinatesPicker)
	{
		_beaverFactory = beaverFactory;
		_inputService = inputService;
		_randomNumberGenerator = randomNumberGenerator;
		_cursorService = cursorService;
		_cursorCoordinatesPicker = cursorCoordinatesPicker;
	}

	public bool ProcessInput()
	{
		if (_inputService.MainMouseButtonDown && !_inputService.MouseOverUI)
		{
			bool isChild = _inputService.IsKeyHeld(SpawnChildKey);
			int count = ((!_inputService.IsKeyHeld(SpawnManyCharactersKey)) ? 1 : ManyBeaversToAdd);
			PlaceBeavers(isChild, count);
			return true;
		}
		return false;
	}

	public void Enter()
	{
		_cursorService.SetCursor(CursorKey);
		_inputService.AddInputProcessor(this);
	}

	public void Exit()
	{
		_cursorService.ResetCursor();
		_inputService.RemoveInputProcessor(this);
	}

	private void PlaceBeavers(bool isChild, int count)
	{
		CursorCoordinates? cursorCoordinates = _cursorCoordinatesPicker.Pick();
		if (!cursorCoordinates.HasValue)
		{
			return;
		}
		Vector3 position = CoordinateSystem.GridToWorldCentered(cursorCoordinates.GetValueOrDefault().TileCoordinates);
		for (int i = 0; i < count; i++)
		{
			float num = _randomNumberGenerator.Range(0f, 1f);
			if (isChild)
			{
				_beaverFactory.CreateChild(position, num);
			}
			else
			{
				_beaverFactory.CreateAdult(position, num);
			}
		}
	}
}
