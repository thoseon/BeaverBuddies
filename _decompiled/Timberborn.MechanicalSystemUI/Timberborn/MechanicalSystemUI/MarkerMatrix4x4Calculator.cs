using Timberborn.Coordinates;
using Timberborn.MechanicalSystem;
using UnityEngine;

namespace Timberborn.MechanicalSystemUI;

internal class MarkerMatrix4x4Calculator
{
	public Matrix4x4 CalculateMatrixFrom(Transput transput)
	{
		return Matrix4x4.TRS(GetPosition(transput), transput.Direction.ToRotation(), Vector3.one);
	}

	private static Vector3 GetPosition(Transput transput)
	{
		Vector3Int coordinates = transput.Coordinates;
		Vector3Int vector3Int = transput.Direction.ToOffset();
		return CoordinateSystem.GridToWorld(new Vector3((float)coordinates.x + (float)vector3Int.x * 0.5f + 0.5f, (float)coordinates.y + (float)vector3Int.y * 0.5f + 0.5f, (float)coordinates.z + (float)vector3Int.z * 0.5f + 0.5f));
	}
}
