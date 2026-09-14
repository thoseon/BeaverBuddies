using System.Text;
using UnityEngine;

namespace Timberborn.Navigation;

internal class NavigationDebuggingService : INavigationDebuggingService
{
	private readonly TerrainFlowFieldCache _terrainFlowFieldCache;

	private readonly RoadFlowFieldCache _roadFlowFieldCache;

	private readonly NodeIdService _nodeIdService;

	public NavigationDebuggingService(TerrainFlowFieldCache terrainFlowFieldCache, RoadFlowFieldCache roadFlowFieldCache, NodeIdService nodeIdService)
	{
		_terrainFlowFieldCache = terrainFlowFieldCache;
		_roadFlowFieldCache = roadFlowFieldCache;
		_nodeIdService = nodeIdService;
	}

	public string InfoAt(Vector3 position)
	{
		if (!_nodeIdService.Contains(position))
		{
			return "Out of map";
		}
		StringBuilder stringBuilder = new StringBuilder();
		int nodeId = _nodeIdService.WorldToId(position);
		AddGeneralInfo(stringBuilder, nodeId);
		AddCachedFlowFieldInfo(stringBuilder, nodeId);
		return stringBuilder.ToString();
	}

	private static void AddGeneralInfo(StringBuilder nodeInfo, int nodeId)
	{
		nodeInfo.Append($"Node id: {nodeId}");
	}

	private void AddCachedFlowFieldInfo(StringBuilder nodeInfo, int nodeId)
	{
		bool flag = _terrainFlowFieldCache.TryGetFlowFieldAtNode(nodeId, out var flowField);
		nodeInfo.Append($"\nCached terrain flow field at position: {flag}");
		if (flag)
		{
			nodeInfo.Append($", number of nodes: {flowField.NumberOfNodes}");
		}
		bool flag2 = _roadFlowFieldCache.TryGetFlowFieldAtNode(nodeId, out var flowField2);
		nodeInfo.Append($"\nCached road flow field at position: {flag2}");
		if (flag2)
		{
			nodeInfo.Append($", number of nodes: {flowField2.NumberOfNodes}");
		}
	}
}
