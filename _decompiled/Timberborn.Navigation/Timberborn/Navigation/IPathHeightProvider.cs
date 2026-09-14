using UnityEngine;

namespace Timberborn.Navigation;

public interface IPathHeightProvider
{
	bool TryGetHeight(Vector3 worldPosition, out float pathHeight);
}
