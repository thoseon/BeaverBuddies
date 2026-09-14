using System;

namespace Timberborn.Multithreading;

public class ParallelizerExceptionLog
{
	public Exception Exception { get; }

	public string ThreadName { get; }

	public ParallelizerExceptionLog(Exception exception, string threadName)
	{
		Exception = exception;
		ThreadName = threadName;
	}
}
