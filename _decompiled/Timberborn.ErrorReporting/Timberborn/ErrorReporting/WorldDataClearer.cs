using Timberborn.SingletonSystem;

namespace Timberborn.ErrorReporting;

internal class WorldDataClearer : IUnloadableSingleton
{
	public void Unload()
	{
		if (!ExceptionListener.AnyUncaughtException)
		{
			WorldDataService.Clear();
		}
	}
}
