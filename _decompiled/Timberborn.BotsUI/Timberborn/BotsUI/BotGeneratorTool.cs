using Timberborn.Bots;
using Timberborn.Coordinates;
using Timberborn.CursorToolSystem;
using Timberborn.InputSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace Timberborn.BotsUI;

public class BotGeneratorTool : IDevModeTool, ITool, IInputProcessor
{
	private static readonly string CursorKey = "BeaverAvatarCursor";

	private static readonly string SpawnManyCharactersKey = "SpawnManyCharacters";

	private static readonly int ManyBotsToAdd = 10;

	private readonly BotFactory _botFactory;

	private readonly InputService _inputService;

	private readonly CursorService _cursorService;

	private readonly CursorCoordinatesPicker _cursorCoordinatesPicker;

	public bool IsDevMode => true;

	public BotGeneratorTool(BotFactory botFactory, InputService inputService, CursorService cursorService, CursorCoordinatesPicker cursorCoordinatesPicker)
	{
		_botFactory = botFactory;
		_inputService = inputService;
		_cursorService = cursorService;
		_cursorCoordinatesPicker = cursorCoordinatesPicker;
	}

	public bool ProcessInput()
	{
		if (_inputService.MainMouseButtonDown && !_inputService.MouseOverUI)
		{
			int count = ((!_inputService.IsKeyHeld(SpawnManyCharactersKey)) ? 1 : ManyBotsToAdd);
			PlaceBots(count);
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

	private void PlaceBots(int count)
	{
		CursorCoordinates? cursorCoordinates = _cursorCoordinatesPicker.Pick();
		if (cursorCoordinates.HasValue)
		{
			Vector3 position = CoordinateSystem.GridToWorldCentered(cursorCoordinates.GetValueOrDefault().TileCoordinates);
			for (int i = 0; i < count; i++)
			{
				_botFactory.Create(position);
			}
		}
	}
}
