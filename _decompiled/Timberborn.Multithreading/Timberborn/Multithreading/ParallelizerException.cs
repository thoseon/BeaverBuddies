using System;
using System.Collections.Immutable;
using System.Text;

namespace Timberborn.Multithreading;

public class ParallelizerException : Exception
{
	public ParallelizerException()
	{
	}

	public ParallelizerException(string message)
		: base(message)
	{
	}

	public ParallelizerException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public static ParallelizerException From(ImmutableArray<ParallelizerExceptionLog> parallelizerExceptionLogs)
	{
		int length = parallelizerExceptionLogs.Length;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < length; i++)
		{
			ParallelizerExceptionLog parallelizerExceptionLog = parallelizerExceptionLogs[i];
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine($"Exception {i + 1}/{length} thrown by {parallelizerExceptionLog.ThreadName}:");
			stringBuilder.AppendLine(parallelizerExceptionLog.Exception.ToString());
		}
		return new ParallelizerException(stringBuilder.ToString());
	}
}
