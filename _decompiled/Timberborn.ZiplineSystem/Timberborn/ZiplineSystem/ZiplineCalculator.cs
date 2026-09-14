using UnityEngine;

namespace Timberborn.ZiplineSystem;

public static class ZiplineCalculator
{
	private static readonly float FrontDistance = 0.38f;

	private static readonly float SideDistance = 0.175f;

	private static readonly float TurningPointOffset = 0.45f;

	public static (Vector3, Vector3) CalculateWorldConnections(Vector3 start, Vector3 end)
	{
		Vector3 normalized = Vector3.ProjectOnPlane((end - start).normalized, Vector3.up).normalized;
		Vector3 vector = start + normalized * FrontDistance;
		Vector3 vector2 = end - normalized * FrontDistance;
		Vector3 vector3 = Vector3.Cross(Vector3.up, normalized).normalized * SideDistance;
		Vector3 item = vector + vector3;
		Vector3 item2 = vector2 + vector3;
		return (item, item2);
	}

	public static (Vector3, Vector3) CalculateGridAnchors(Vector3 start, Vector3 end)
	{
		Vector3 normalized = Vector3.ProjectOnPlane((end - start).normalized, new Vector3(0f, 0f, 1f)).normalized;
		Vector3 item = start + normalized * FrontDistance;
		Vector3 item2 = end - normalized * FrontDistance;
		return (item, item2);
	}

	public static Vector3 CalculateTurn(Vector3 turnStart, Vector3 turnEnd, Vector3 start)
	{
		Vector3 item = CalculateWorldConnections(turnStart, turnEnd).Item1;
		Vector3 normalized = new Vector3(item.z - start.z, 0f, start.x - item.x).normalized;
		return turnStart + normalized * TurningPointOffset;
	}
}
