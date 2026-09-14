using System.Threading;
using Timberborn.SingletonSystem;

namespace Timberborn.Multithreading;

internal class MainThreadPrioritySetter : ILoadableSingleton
{
	public void Load()
	{
		Thread.CurrentThread.Priority = ThreadPriority.Highest;
	}
}
