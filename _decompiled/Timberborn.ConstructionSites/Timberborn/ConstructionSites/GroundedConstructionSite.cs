using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.ConstructionSites;

public class GroundedConstructionSite : BaseComponent, IAwakableComponent, IUnfinishedStateListener, IFinishedStateListener, IPostPlacementChangeListener, IConstructionSiteValidator
{
	private readonly IBlockService _blockService;

	private readonly MatterBelowValidator _matterBelowValidator;

	private BlockObject _blockObject;

	public bool IsValid { get; private set; } = true;

	public bool IsModelValid => IsValid;

	public event EventHandler ValidationStateChanged;

	public GroundedConstructionSite(IBlockService blockService, MatterBelowValidator matterBelowValidator)
	{
		_blockService = blockService;
		_matterBelowValidator = matterBelowValidator;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void OnEnterFinishedState()
	{
		UpdateConstructionSitesAtop();
	}

	public void OnExitFinishedState()
	{
	}

	public void OnEnterUnfinishedState()
	{
		Validate();
	}

	public void OnExitUnfinishedState()
	{
	}

	public void OnPostPlacementChanged()
	{
		if (_blockObject.IsPreview)
		{
			Validate();
		}
	}

	public void Validate()
	{
		bool isValid = IsValid;
		IsValid = (from block in _blockObject.PositionedBlocks.GetOccupiedBlocks()
			where block.MatterBelow.IsSolidMatter()
			where block.Coordinates.z == _blockObject.CoordinatesAtBaseZ.z
			select block).All((Block block) => BlockIsGrounded(in block));
		if (IsValid != isValid)
		{
			ValidationStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void UpdateConstructionSitesAtop()
	{
		foreach (Block item in from block in _blockObject.PositionedBlocks.GetOccupiedBlocks()
			where block.Stackable.IsStackable()
			select block)
		{
			UpdateConstructionSitesAtopBlock(item.Coordinates);
		}
	}

	private void UpdateConstructionSitesAtopBlock(Vector3Int coordinates)
	{
		Vector3Int coordinates2 = coordinates + new Vector3Int(0, 0, 1);
		foreach (BlockObject item in _blockService.GetObjectsAt(coordinates2))
		{
			if (item.IsUnfinished)
			{
				item.GetComponent<GroundedConstructionSite>()?.Validate();
			}
		}
	}

	private bool BlockIsGrounded(in Block block)
	{
		return _matterBelowValidator.ValidateIgnoringUnfinishedStackable(in block);
	}
}
