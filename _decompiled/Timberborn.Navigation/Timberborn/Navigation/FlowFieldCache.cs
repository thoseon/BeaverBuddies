using System;
using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Navigation;

internal class FlowFieldCache
{
	private class CacheEntry
	{
		public AccessFlowField AccessFlowField { get; } = new AccessFlowField();

		public int NumberOfCachers { get; set; }
	}

	private readonly Dictionary<int, CacheEntry> _flowFields = new Dictionary<int, CacheEntry>();

	public AccessFlowField GetFlowFieldAtNode(int nodeId)
	{
		if (!TryGetFlowFieldAtNode(nodeId, out var flowField))
		{
			throw new InvalidOperationException($"There's no cached flow field at {nodeId}");
		}
		return flowField;
	}

	public bool TryGetFlowFieldAtNode(int nodeId, out AccessFlowField flowField)
	{
		if (TryGetCacheEntry(nodeId, out var cacheEntry))
		{
			flowField = cacheEntry.AccessFlowField;
			return true;
		}
		flowField = null;
		return false;
	}

	public void StartCachingAtNode(int nodeId)
	{
		if (!TryGetCacheEntry(nodeId, out var cacheEntry))
		{
			cacheEntry = new CacheEntry();
			_flowFields[nodeId] = cacheEntry;
		}
		CacheEntry cacheEntry2 = cacheEntry;
		int numberOfCachers = cacheEntry2.NumberOfCachers + 1;
		cacheEntry2.NumberOfCachers = numberOfCachers;
	}

	public void StopCachingAtNode(int nodeId)
	{
		if (!TryGetCacheEntry(nodeId, out var cacheEntry))
		{
			throw new InvalidOperationException($"Can't stop caching at {nodeId}. There's no cached flow field there.");
		}
		if (cacheEntry.NumberOfCachers == 0)
		{
			throw new InvalidOperationException($"Can't decrement cachers at {nodeId}, it's already 0.");
		}
		CacheEntry cacheEntry2 = cacheEntry;
		int numberOfCachers = cacheEntry2.NumberOfCachers - 1;
		cacheEntry2.NumberOfCachers = numberOfCachers;
		if (cacheEntry.NumberOfCachers == 0)
		{
			_flowFields.Remove(nodeId);
		}
	}

	public void OnNodesChanged(ReadOnlyList<int> nodeIds)
	{
		foreach (CacheEntry value in _flowFields.Values)
		{
			value.AccessFlowField.OnNodesChanged(nodeIds);
		}
	}

	private bool TryGetCacheEntry(int nodeId, out CacheEntry cacheEntry)
	{
		return _flowFields.TryGetValue(nodeId, out cacheEntry);
	}
}
