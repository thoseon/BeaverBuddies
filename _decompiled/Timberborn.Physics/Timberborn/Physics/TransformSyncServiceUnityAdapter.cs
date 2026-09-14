using UnityEngine;

namespace Timberborn.Physics;

internal class TransformSyncServiceUnityAdapter : MonoBehaviour
{
	public void LateUpdate()
	{
		UnityEngine.Physics.SyncTransforms();
	}
}
