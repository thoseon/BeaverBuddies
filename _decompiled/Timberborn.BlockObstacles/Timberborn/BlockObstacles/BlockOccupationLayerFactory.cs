using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.BlockObstacles;

public class BlockOccupationLayerFactory : ILoadableSingleton
{
	private readonly BlockObjectFactory _blockObjectFactory;

	private readonly TemplateService _templateService;

	private BlockObjectSpec _blockOccupierTemplate;

	public BlockOccupationLayerFactory(BlockObjectFactory blockObjectFactory, TemplateService templateService)
	{
		_blockObjectFactory = blockObjectFactory;
		_templateService = templateService;
	}

	public void Load()
	{
		_blockOccupierTemplate = _templateService.GetSingle<BlockOccupierSpec>().GetSpec<BlockObjectSpec>();
	}

	public BlockOccupationLayer Create(Transform parent, Vector3 anchorPosition, int gridHeight, Vector2 layerSize)
	{
		BlockOccupationLayer blockOccupationLayer = new BlockOccupationLayer(gridHeight);
		for (int i = 0; (float)i < layerSize.x; i++)
		{
			for (int j = 0; (float)j < layerSize.y; j++)
			{
				Vector3 occupierLocalPosition = new Vector3(anchorPosition.x + (float)i, 0f, anchorPosition.z + (float)j);
				BlockOccupier blockOccupier = CreateBlockOccupier(parent, gridHeight, occupierLocalPosition);
				blockOccupationLayer.AddBlockOccupier(blockOccupier);
			}
		}
		return blockOccupationLayer;
	}

	private BlockOccupier CreateBlockOccupier(Transform parent, int gridHeight, Vector3 occupierLocalPosition)
	{
		Vector3Int occupierWorldGridPosition = GetOccupierWorldGridPosition(parent, gridHeight, occupierLocalPosition);
		return CreateBlockOccupier(parent, occupierWorldGridPosition);
	}

	private static Vector3Int GetOccupierWorldGridPosition(Transform parent, int layerHeight, Vector3 localBlockOccupierPosition)
	{
		Vector3Int result = CoordinateSystem.WorldToGridInt(parent.TransformPoint(localBlockOccupierPosition));
		result.z = layerHeight;
		return result;
	}

	private BlockOccupier CreateBlockOccupier(Transform parent, Vector3Int worldBlockOccupierGridPosition)
	{
		BlockObject blockObject = _blockObjectFactory.CreateAsPreview(_blockOccupierTemplate, parent, new Placement(worldBlockOccupierGridPosition));
		blockObject.GameObject.name = string.Format("{0} {1}", "BlockOccupier", worldBlockOccupierGridPosition);
		return blockObject.GetComponent<BlockOccupier>();
	}
}
