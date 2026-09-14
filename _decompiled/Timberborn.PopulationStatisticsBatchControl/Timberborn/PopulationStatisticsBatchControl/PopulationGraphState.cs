using System.Collections.Generic;

namespace Timberborn.PopulationStatisticsBatchControl;

internal class PopulationGraphState
{
	private readonly Dictionary<string, bool> _graphStates = new Dictionary<string, bool>();

	public bool GetState(string graphId)
	{
		if (HasState(graphId))
		{
			return _graphStates[graphId];
		}
		return false;
	}

	public bool HasState(string graphId)
	{
		return _graphStates.ContainsKey(graphId);
	}

	public void SetState(string graphId, bool enabled)
	{
		_graphStates[graphId] = enabled;
	}
}
