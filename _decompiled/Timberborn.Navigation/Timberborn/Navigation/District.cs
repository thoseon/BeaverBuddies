using UnityEngine;

namespace Timberborn.Navigation;

public class District
{
	private readonly Vector3Int _centerCoordinates;

	internal int CenterNodeId { get; }

	internal District(int centerNodeId, Vector3Int centerCoordinates)
	{
		CenterNodeId = centerNodeId;
		_centerCoordinates = centerCoordinates;
	}

	public override string ToString()
	{
		return string.Format("{0}: {1}, ", "_centerCoordinates", _centerCoordinates) + string.Format("{0}: {1}", "CenterNodeId", CenterNodeId);
	}
}
