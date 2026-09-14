using System;

namespace Timberborn.MapSystem;

public class MapSaverException : Exception
{
	public MapSaverException()
	{
	}

	public MapSaverException(string message)
		: base(message)
	{
	}

	public MapSaverException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
