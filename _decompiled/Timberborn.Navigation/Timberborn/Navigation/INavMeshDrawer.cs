using UnityEngine;

namespace Timberborn.Navigation;

public interface INavMeshDrawer
{
	void DrawForOneFrameAroundCoordinates(Vector3Int coordinates);
}
