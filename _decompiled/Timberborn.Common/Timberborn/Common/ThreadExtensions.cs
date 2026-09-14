using System.Threading;

namespace Timberborn.Common;

public static class ThreadExtensions
{
	public static string DisplayName(this Thread thread)
	{
		if (thread != null)
		{
			return thread.Name ?? thread.ManagedThreadId.ToString();
		}
		return string.Empty;
	}
}
