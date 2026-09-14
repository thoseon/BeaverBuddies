using Timberborn.AssetSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.StatusSystem;

public class StatusIconCyclerFactory
{
	private static readonly string IconCyclerPrefabPath = "UI/Statuses/StatusIconCycler";

	private static readonly float YOffset = 1f;

	private readonly BoundsCalculator _boundsCalculator;

	private readonly IAssetLoader _assetLoader;

	private GameObject _statusIconCyclerPrefab;

	public StatusIconCyclerFactory(BoundsCalculator boundsCalculator, IAssetLoader assetLoader)
	{
		_boundsCalculator = boundsCalculator;
		_assetLoader = assetLoader;
	}

	public GameObject CreateAsChild(Transform parent)
	{
		Vector3 position = parent.position;
		position.y = _boundsCalculator.GetRendererYMaxBound(parent) + YOffset;
		GameObject gameObject = Object.Instantiate(GetPrefab(), parent);
		gameObject.transform.position = position;
		return gameObject;
	}

	private GameObject GetPrefab()
	{
		if (!_statusIconCyclerPrefab)
		{
			_statusIconCyclerPrefab = _assetLoader.Load<GameObject>(IconCyclerPrefabPath);
		}
		return _statusIconCyclerPrefab;
	}
}
