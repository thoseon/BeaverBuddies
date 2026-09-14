using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.TubeSystem;

internal class TubePlatform : BaseComponent, IAwakableComponent, IInitializableEntity, IModelUpdater
{
	private readonly IBlockService _blockService;

	private readonly PreviewBlockService _previewBlockService;

	private readonly StackableBlockService _stackableBlockService;

	private BlockObject _blockObject;

	private GameObject _tubePlatform;

	public TubePlatform(IBlockService blockService, PreviewBlockService previewBlockService, StackableBlockService stackableBlockService)
	{
		_blockService = blockService;
		_previewBlockService = previewBlockService;
		_stackableBlockService = stackableBlockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		TubePlatformSpec component = GetComponent<TubePlatformSpec>();
		_tubePlatform = base.GameObject.FindChild(component.PlatformModelName);
	}

	public void InitializeEntity()
	{
		UpdateVisibility();
	}

	public void UpdateModel()
	{
		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		Vector3Int vector3Int = _blockObject.Coordinates.Above();
		Tube pathObjectComponentAt = _blockService.GetPathObjectComponentAt<Tube>(vector3Int);
		Tube pathObjectComponentAt2 = _previewBlockService.GetPathObjectComponentAt<Tube>(vector3Int);
		bool flag = _stackableBlockService.IsStackableBlockAt(vector3Int);
		_tubePlatform.SetActive((!pathObjectComponentAt && !pathObjectComponentAt2) || !flag);
	}
}
