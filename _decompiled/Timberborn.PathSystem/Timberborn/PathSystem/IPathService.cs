using UnityEngine;

namespace Timberborn.PathSystem;

public interface IPathService
{
	bool IsPath(Vector3Int coordinates);
}
