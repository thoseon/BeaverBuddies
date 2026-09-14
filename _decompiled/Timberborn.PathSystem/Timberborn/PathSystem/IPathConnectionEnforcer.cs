using UnityEngine;

namespace Timberborn.PathSystem;

public interface IPathConnectionEnforcer
{
	bool CanConnectPath(Vector3Int origin, Vector3Int target);
}
