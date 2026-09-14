using System;

namespace Timberborn.Persistence;

public class ObsoleteValueException : Exception
{
	public ObsoleteValueException()
	{
	}

	public ObsoleteValueException(string message)
		: base(message)
	{
	}

	public ObsoleteValueException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
