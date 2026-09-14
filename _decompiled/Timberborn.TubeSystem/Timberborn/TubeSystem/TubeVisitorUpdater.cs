using Timberborn.SingletonSystem;

namespace Timberborn.TubeSystem;

internal class TubeVisitorUpdater : IUpdatableSingleton
{
	private readonly TubeVisitorRegistry _tubeVisitorRegistry;

	private readonly TubeMap _tubeMap;

	public TubeVisitorUpdater(TubeVisitorRegistry tubeVisitorRegistry, TubeMap tubeMap)
	{
		_tubeVisitorRegistry = tubeVisitorRegistry;
		_tubeMap = tubeMap;
	}

	public void UpdateSingleton()
	{
		if (!_tubeMap.AnyTubeBuilt)
		{
			return;
		}
		foreach (TubeVisitor tubeVisitor in _tubeVisitorRegistry.TubeVisitors)
		{
			tubeVisitor.UpdateVisit();
		}
	}
}
