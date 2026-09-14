namespace Timberborn.Navigation;

internal interface INavMeshGraph
{
	void ConnectNodes(int aNodeId, int bNodeId, int groupId, float cost);

	void DisconnectNodes(int aNodeId, int bNodeId);
}
