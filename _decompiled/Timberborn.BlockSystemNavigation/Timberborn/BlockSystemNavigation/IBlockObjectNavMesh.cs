using Timberborn.Navigation;

namespace Timberborn.BlockSystemNavigation;

public interface IBlockObjectNavMesh
{
	NavMeshObject NavMeshObject { get; }

	void RecalculateNavMeshObject();
}
