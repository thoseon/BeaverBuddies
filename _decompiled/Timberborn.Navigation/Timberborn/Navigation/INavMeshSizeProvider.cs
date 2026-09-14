using UnityEngine;

namespace Timberborn.Navigation;

public interface INavMeshSizeProvider
{
	Vector3Int Size { get; }
}
