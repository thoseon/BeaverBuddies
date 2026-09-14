using System;

namespace Timberborn.GameSaveRuntimeSystem;

public class GameSaverException : Exception
{
	public GameSaverException()
	{
	}

	public GameSaverException(string message)
		: base(message)
	{
	}

	public GameSaverException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
