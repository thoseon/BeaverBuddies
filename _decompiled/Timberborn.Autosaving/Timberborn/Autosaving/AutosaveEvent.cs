using Timberborn.GameSaveRuntimeSystem;

namespace Timberborn.Autosaving;

public class AutosaveEvent
{
	public bool Successful { get; }

	public GameSaverException Exception { get; }

	private AutosaveEvent(bool successful, GameSaverException exception)
	{
		Successful = successful;
		Exception = exception;
	}

	public static AutosaveEvent CreateSuccess()
	{
		return new AutosaveEvent(successful: true, null);
	}

	public static AutosaveEvent CreateFailure(GameSaverException exception)
	{
		return new AutosaveEvent(successful: false, exception);
	}
}
