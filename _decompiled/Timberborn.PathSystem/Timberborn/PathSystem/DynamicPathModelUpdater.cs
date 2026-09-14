using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.PathSystem;

public class DynamicPathModelUpdater : BaseComponent, IAwakableComponent, IDeletableEntity, IPreviewSelectionListener, IPrePlacementChangeListener, IPostPlacementChangeListener
{
	private readonly IBlockService _blockService;

	private BlockObject _blockObject;

	public DynamicPathModelUpdater(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void DeleteEntity()
	{
		UpdatePathModel();
	}

	public void OnPrePlacementChanged()
	{
		UpdatePathModel();
	}

	public void OnPostPlacementChanged()
	{
		UpdatePathModel();
	}

	public void OnPreviewSelect()
	{
		UpdatePathModel();
	}

	public void OnPreviewUnselect()
	{
		UpdatePathModel();
	}

	private void UpdatePathModel()
	{
		if (!_blockObject.Positioned)
		{
			return;
		}
		foreach (Vector3Int foundationCoordinate in _blockObject.PositionedBlocks.GetFoundationCoordinates())
		{
			DynamicPathModel pathObjectComponentAt = _blockService.GetPathObjectComponentAt<DynamicPathModel>(foundationCoordinate);
			if ((bool)pathObjectComponentAt)
			{
				pathObjectComponentAt.UpdateModel();
			}
		}
	}
}
