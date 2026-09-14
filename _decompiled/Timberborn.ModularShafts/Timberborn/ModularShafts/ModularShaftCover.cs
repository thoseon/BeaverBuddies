using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.ModularShafts;

internal class ModularShaftCover : BaseComponent, IAwakableComponent
{
	private readonly IBlockService _blockService;

	private readonly PreviewBlockService _previewBlockService;

	private BlockObject _blockObject;

	private GameObject _cover;

	public ModularShaftCover(IBlockService blockService, PreviewBlockService previewBlockService)
	{
		_blockService = blockService;
		_previewBlockService = previewBlockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		ModularShaftCoverSpec component = GetComponent<ModularShaftCoverSpec>();
		_cover = base.GameObject.FindChild(component.CoverModelName);
	}

	public void UpdateModel()
	{
		Vector3Int coordinates = _blockObject.Coordinates.Above();
		ModularShaft bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<ModularShaft>(coordinates);
		ModularShaft bottomObjectComponentAt2 = _previewBlockService.GetBottomObjectComponentAt<ModularShaft>(coordinates);
		_cover.SetActive(!bottomObjectComponentAt && !bottomObjectComponentAt2);
	}
}
