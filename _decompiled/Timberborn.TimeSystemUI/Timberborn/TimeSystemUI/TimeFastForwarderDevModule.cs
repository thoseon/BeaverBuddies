using Timberborn.Debugging;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.TimeSystemUI;

internal class TimeFastForwarderDevModule : IDevModule, IInputProcessor, ILoadableSingleton
{
	private static readonly string JumpToNextDaytimeKey = "JumpToNextDaytime";

	private readonly InputService _inputService;

	private readonly TimeFastForwarder _timeFastForwarder;

	public TimeFastForwarderDevModule(InputService inputService, TimeFastForwarder timeFastForwarder)
	{
		_inputService = inputService;
		_timeFastForwarder = timeFastForwarder;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.CreateBindable("Jump to next daytime", JumpToNextDaytimeKey, _timeFastForwarder.JumpToNextDaytime)).Build();
	}

	public void Load()
	{
		_inputService.AddInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(JumpToNextDaytimeKey))
		{
			_timeFastForwarder.JumpToNextDaytime();
			return true;
		}
		return false;
	}
}
