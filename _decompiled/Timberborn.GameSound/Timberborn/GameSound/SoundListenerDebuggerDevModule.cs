using Timberborn.Debugging;

namespace Timberborn.GameSound;

internal class SoundListenerDebuggerDevModule : IDevModule
{
	private readonly SoundListenerDebugger _soundListenerDebugger;

	public SoundListenerDebuggerDevModule(SoundListenerDebugger soundListenerDebugger)
	{
		_soundListenerDebugger = soundListenerDebugger;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle sound listener debugger", ToggleDebugger)).Build();
	}

	private void ToggleDebugger()
	{
		_soundListenerDebugger.ToggleActive();
	}
}
