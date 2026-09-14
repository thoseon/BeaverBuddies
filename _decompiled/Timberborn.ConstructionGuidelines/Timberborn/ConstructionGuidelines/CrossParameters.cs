using UnityEngine;

namespace Timberborn.ConstructionGuidelines;

internal record CrossParameters
{
	public Vector3Int Center { get; private set; } = new Vector3Int(-1, -1, -1);

	public Vector2Int Min { get; private set; } = new Vector2Int(-1, -1);

	public Vector2Int Max { get; private set; } = new Vector2Int(-1, -1);

	private bool _isFromPreview;

	public bool CrossParametersUpdated(Vector3Int center, Vector2Int min, Vector2Int max, bool isFromPreview)
	{
		if (Center != center || Min != min || Max != max || _isFromPreview != isFromPreview)
		{
			Center = center;
			Min = min;
			Max = max;
			_isFromPreview = isFromPreview;
			return true;
		}
		return false;
	}

	public void Reset()
	{
		Center = new Vector3Int(-1, -1, -1);
		Min = new Vector2Int(-1, -1);
		Max = new Vector2Int(-1, -1);
	}
}
