using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.PathSystem;

public interface IConnectionService
{
	bool CanConnectInDirection(Vector3Int origin, Direction2D direction2D);
}
