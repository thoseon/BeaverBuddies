using System.Collections.Generic;
using Timberborn.AreaSelectionSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.LevelVisibilitySystem;
using Timberborn.PrefabOptimization;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal class BoundsNavRangeCalculator
{
	private readonly IBlockService _blockService;

	private readonly PreviewBlockService _previewBlockService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly HashSet<Vector3Int> _area = new HashSet<Vector3Int>();

	public BoundsNavRangeCalculator(IBlockService blockService, PreviewBlockService previewBlockService, ILevelVisibilityService levelVisibilityService)
	{
		_blockService = blockService;
		_previewBlockService = previewBlockService;
		_levelVisibilityService = levelVisibilityService;
	}

	public void Recalculate(IReadOnlyCollection<Vector3Int> area, NeighboredValues8<IntermediateMesh> meshes, BoundsMesh boundsMesh)
	{
		_area.AddRange(area);
		foreach (Vector3Int item in area)
		{
			if (!AnyBlockingObjectAt(item) && _levelVisibilityService.BlockIsVisible(item))
			{
				AddToBounds(item, meshes, boundsMesh);
			}
		}
		_area.Clear();
	}

	private bool AnyBlockingObjectAt(Vector3Int coordinates)
	{
		if (!IsBlockingObject(_blockService.GetBottomObjectAt(coordinates)))
		{
			return IsBlockingObject(_previewBlockService.GetBottomPreviewAt(coordinates));
		}
		return true;
	}

	private static bool IsBlockingObject(BlockObject blockObject)
	{
		if ((bool)blockObject)
		{
			return blockObject.GetEnabledComponent<AreaBoundsDrawingBlocker>();
		}
		return false;
	}

	private void AddToBounds(Vector3Int coordinates, NeighboredValues8<IntermediateMesh> meshes, BoundsMesh boundsMesh)
	{
		bool flag = IsVisibleSide(coordinates, new Vector3Int(0, -1, 0));
		bool flag2 = IsVisibleSide(coordinates, new Vector3Int(-1, -1, 0));
		bool flag3 = IsVisibleSide(coordinates, new Vector3Int(-1, 0, 0));
		bool flag4 = IsVisibleSide(coordinates, new Vector3Int(-1, 1, 0));
		bool flag5 = IsVisibleSide(coordinates, new Vector3Int(0, 1, 0));
		bool flag6 = IsVisibleSide(coordinates, new Vector3Int(1, 1, 0));
		bool flag7 = IsVisibleSide(coordinates, new Vector3Int(1, 0, 0));
		bool flag8 = IsVisibleSide(coordinates, new Vector3Int(1, -1, 0));
		if (flag | flag2 | flag3 | flag4 | flag5 | flag6 | flag7 | flag8)
		{
			IntermediateMesh value = meshes.GetMatch(flag, flag2, flag3, flag4, flag5, flag6, flag7, flag8).Value;
			Vector3 translation = CoordinateSystem.GridToWorldCentered(coordinates);
			boundsMesh.Append(coordinates.z, value, new TranslationTransform(translation));
		}
	}

	private bool IsVisibleSide(Vector3Int coordinates, Vector3Int neighborDelta)
	{
		return !_area.Contains(coordinates + neighborDelta);
	}
}
