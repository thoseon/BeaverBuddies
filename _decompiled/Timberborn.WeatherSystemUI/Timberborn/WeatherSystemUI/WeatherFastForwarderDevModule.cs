using Timberborn.Debugging;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using Timberborn.WeatherSystem;

namespace Timberborn.WeatherSystemUI;

internal class WeatherFastForwarderDevModule : IDevModule, IInputProcessor, ILoadableSingleton
{
	private static readonly string JumpToNextSeasonKey = "JumpToNextSeason";

	private readonly InputService _inputService;

	private readonly WeatherFastForwarder _weatherFastForwarder;

	public WeatherFastForwarderDevModule(InputService inputService, WeatherFastForwarder weatherFastForwarder)
	{
		_inputService = inputService;
		_weatherFastForwarder = weatherFastForwarder;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.CreateBindable("Jump to next season", JumpToNextSeasonKey, _weatherFastForwarder.JumpToNextSeason)).Build();
	}

	public void Load()
	{
		_inputService.AddInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(JumpToNextSeasonKey))
		{
			_weatherFastForwarder.JumpToNextSeason();
			return true;
		}
		return false;
	}
}
