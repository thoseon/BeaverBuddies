using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.TubeSystem;

internal class TubeVisitorRegistry
{
	private readonly List<TubeVisitor> _tubeVisitors = new List<TubeVisitor>();

	public ReadOnlyList<TubeVisitor> TubeVisitors => _tubeVisitors.AsReadOnlyList();

	public void Register(TubeVisitor tubeVisitor)
	{
		_tubeVisitors.Add(tubeVisitor);
	}

	public void Unregister(TubeVisitor tubeVisitor)
	{
		_tubeVisitors.Remove(tubeVisitor);
	}
}
