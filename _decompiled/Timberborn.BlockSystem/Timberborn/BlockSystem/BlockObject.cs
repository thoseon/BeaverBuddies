using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.ErrorReporting;
using Timberborn.Persistence;
using Timberborn.TemplateSystem;
using Timberborn.TransformControl;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class BlockObject : BaseComponent, IAwakableComponent, IPreInitializableEntity, IPersistentEntity, IDeletableEntity
{
	private static readonly ComponentKey BlockObjectKey = new ComponentKey("BlockObject");

	private static readonly PropertyKey<Vector3Int> CoordinatesKey = new PropertyKey<Vector3Int>("Coordinates");

	private static readonly PropertyKey<Orientation> OrientationKey = new PropertyKey<Orientation>("Orientation");

	private static readonly PropertyKey<bool> FlippedKey = new PropertyKey<bool>("Flipped");

	private static readonly string BlockObjectLoadingIssueLocKey = "LoadingIssue.BlockObjectLoadingIssue";

	private readonly BlockService _blockService;

	private readonly BlockValidator _blockValidator;

	private readonly OverridenBlockObjectService _overridenBlockObjectService;

	private readonly EntityService _entityService;

	private readonly BlockObjectValidationService _blockObjectValidationService;

	private readonly ILoadingIssueService _loadingIssueService;

	private PositionModifier _positionModifier;

	private RotationModifier _rotationModifier;

	private ScaleModifier _scaleModifier;

	private PlacementChangeNotifier _placementChangeNotifier;

	private Blocks _blocks;

	private BlockObjectCenter _blockObjectCenter;

	private BlockObjectState _blockObjectState;

	private BlockObjectSpec _blockObjectSpec;

	private readonly List<IBlockObjectDeletionBlocker> _deletionBlockers = new List<IBlockObjectDeletionBlocker>();

	public Vector3Int Coordinates { get; private set; }

	public Vector3Int CoordinatesAtBaseZ { get; private set; }

	public Orientation Orientation { get; private set; }

	public FlipMode FlipMode { get; private set; }

	public PositionedBlocks PositionedBlocks { get; private set; }

	public PositionedEntrance PositionedEntrance { get; private set; }

	public bool Solid { get; private set; }

	public bool GroundOnly { get; private set; }

	public bool AboveGround { get; private set; }

	public bool AddedToService { get; private set; }

	public bool Overridable { get; private set; }

	public Blocks Blocks => _blocks ?? (_blocks = _blockObjectSpec.GetBlocks());

	public EntranceBlockSpec Entrance => _blockObjectSpec.Entrance;

	public int BaseZ => _blockObjectSpec.BaseZ;

	public Placement Placement => new Placement(Coordinates, Orientation, FlipMode);

	public bool HasEntrance => PositionedEntrance != null;

	public bool Positioned => PositionedBlocks != null;

	public bool IsFinished => _blockObjectState.IsFinished;

	public bool IsUnfinished => _blockObjectState.IsUnfinished;

	public bool IsPreview => _blockObjectState.IsPreview;

	public event EventHandler<bool> OverridableChanged;

	internal BlockObject(BlockService blockService, BlockValidator blockValidator, OverridenBlockObjectService overridenBlockObjectService, EntityService entityService, BlockObjectValidationService blockObjectValidationService, ILoadingIssueService loadingIssueService)
	{
		_blockService = blockService;
		_blockValidator = blockValidator;
		_overridenBlockObjectService = overridenBlockObjectService;
		_entityService = entityService;
		_blockObjectValidationService = blockObjectValidationService;
		_loadingIssueService = loadingIssueService;
	}

	public void Awake()
	{
		_placementChangeNotifier = GetComponent<PlacementChangeNotifier>();
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_blockObjectState = GetComponent<BlockObjectState>();
		_blockObjectSpec = GetComponent<BlockObjectSpec>();
		GetComponents(_deletionBlockers);
		TransformController component = GetComponent<TransformController>();
		_positionModifier = component.AddPositionModifier();
		_rotationModifier = component.AddRotationModifier(0);
		_scaleModifier = component.AddScaleModifier();
		Overridable = _blockObjectSpec.Overridable;
		if (!Blocks.GetAllBlocks().Any())
		{
			throw new Exception(base.Name + " BlockObjectSpec Blocks must contain at least one element");
		}
		Solid = Blocks.GetOccupiedBlocks().Any((Block block) => block.Stackable != BlockStackable.None);
		GroundOnly = Blocks.GetAllBlocks().Any((Block block) => block.MatterBelow == MatterBelow.Ground);
		AboveGround = Blocks.GetAllBlocks().Any((Block block) => block.MatterBelow == MatterBelow.Stackable);
	}

	public void PreInitializeEntity()
	{
		if (TryGetComponent<BlockObjectInit>(out var component))
		{
			Reposition(component.Placement);
			if (component.MarkFinished)
			{
				MarkAsFinished();
			}
		}
	}

	public void DeleteEntity()
	{
		RemoveFromService();
	}

	public void MarkAsFinished()
	{
		_blockObjectState.MarkAsFinished();
	}

	public void MarkAsFinishedAndAddToServices()
	{
		_blockObjectState.MarkAsFinished();
		if (!AddedToService)
		{
			AddToService();
		}
	}

	public void MarkAsPreview()
	{
		_blockObjectState.MarkAsPreview();
		if (AddedToService)
		{
			RemoveFromService();
		}
	}

	public void MarkAsPreviewAndInitialize()
	{
		_blockObjectState.MarkAsPreview();
		_blockObjectState.Initialize();
		if (AddedToService)
		{
			RemoveFromService();
		}
	}

	public void MakeOverridable()
	{
		ToggleOverridable(isOverridable: true);
	}

	public void MakeNonOverridable()
	{
		ToggleOverridable(isOverridable: false);
	}

	public void Reposition(Placement placement)
	{
		if (Placement != placement || !Positioned)
		{
			_placementChangeNotifier.NotifyPreChangeListeners();
			RemoveFromService();
			UpdateValues(placement);
			AddToService();
			_placementChangeNotifier.NotifyPostChangeListeners();
		}
	}

	public bool CanDelete()
	{
		return !_deletionBlockers.FastAny((IBlockObjectDeletionBlocker blocker) => blocker.IsDeletionBlocked);
	}

	public Vector2Int TransformTile(Vector2Int tile)
	{
		return Blocks.Transform(tile, Placement);
	}

	public Vector3Int TransformCoordinates(Vector3Int coordinates)
	{
		return Blocks.Transform(coordinates, Placement);
	}

	public Vector3 TransformCoordinates(Vector3 coordinates)
	{
		return Blocks.Transform(coordinates, Placement);
	}

	public Direction2D TransformDirection(Direction2D direction2D)
	{
		return Orientation.Transform(FlipMode.Transform(direction2D));
	}

	public Direction3D TransformDirection(Direction3D direction3D)
	{
		return FlipMode.Transform(direction3D).RotateHorizontally(Orientation);
	}

	public bool IsIntersecting(Block block)
	{
		return PositionedBlocks.HasIntersectingBlock(block);
	}

	public Vector3Int CoordinatesBehind()
	{
		return TransformCoordinates(new Vector3Int(0, _blockObjectSpec.Size.y, 0));
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(BlockObjectKey);
		component.Set(CoordinatesKey, Coordinates + new Vector3Int(0, 0, BaseZ));
		if (Orientation != Orientation.Cw0)
		{
			component.Set(OrientationKey, Orientation);
		}
		if (FlipMode.IsFlipped)
		{
			component.Set(FlippedKey, value: true);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(BlockObjectKey, out var objectLoader))
		{
			Coordinates = objectLoader.Get(CoordinatesKey) - new Vector3Int(0, 0, BaseZ);
			Orientation = (objectLoader.Has(OrientationKey) ? objectLoader.Get(OrientationKey) : Orientation.Cw0);
			FlipMode = new FlipMode(_blockObjectSpec.Flippable && objectLoader.Has(FlippedKey) && objectLoader.Get(FlippedKey));
		}
		UpdateValues(Placement);
	}

	public bool IsAlmostValid()
	{
		return _blockValidator.BlocksAlmostValid(PositionedBlocks);
	}

	public bool IsValid()
	{
		if (_blockValidator.BlocksValid(PositionedBlocks))
		{
			return _blockObjectValidationService.IsValid(this);
		}
		return false;
	}

	public void AddToServiceAfterLoad()
	{
		if (!AddedToService)
		{
			if (IsValid() && GetComponent<TemplateSpec>().UsableWithCurrentFeatureToggles)
			{
				AddToService();
				return;
			}
			LabeledEntitySpec component = GetComponent<LabeledEntitySpec>();
			_loadingIssueService.AddIssue(string.Format("Can't validate loaded {0} {1} at {2}.", "BlockObject", base.Name, Coordinates) + " It's not backward compatible. Deleting it.", BlockObjectLoadingIssueLocKey, component.DisplayNameLocKey, paramIsLocKey: true);
			_entityService.Delete(this);
		}
	}

	private void AddToService()
	{
		if (!AddedToService && !IsPreview)
		{
			if (!IsValid())
			{
				throw new InvalidOperationException(string.Format("Cannot place {0} {1} at {2}.", "BlockObject", base.Name, Coordinates));
			}
			_overridenBlockObjectService.OverrideBlockObjects(this);
			_blockService.SetObject(this);
			AddedToService = true;
		}
	}

	private void UpdateValues(Placement placement)
	{
		Coordinates = placement.Coordinates;
		Orientation = placement.Orientation;
		FlipMode = placement.FlipMode;
		UpdateTransformedBlocks();
		UpdateTransform();
		_blockObjectCenter.UpdateCenter();
	}

	private void RemoveFromService()
	{
		if (AddedToService)
		{
			_blockService.UnsetObject(this);
			AddedToService = false;
		}
	}

	private void ToggleOverridable(bool isOverridable)
	{
		bool addedToService = AddedToService;
		RemoveFromService();
		Overridable = isOverridable;
		if (addedToService)
		{
			AddToService();
		}
		OverridableChanged?.Invoke(this, isOverridable);
	}

	private void UpdateTransformedBlocks()
	{
		PositionedBlocks = PositionedBlocks.From(Blocks, Placement);
		PositionedEntrance = PositionedEntrance.From(Blocks, Entrance, Placement);
		CoordinatesAtBaseZ = Coordinates + new Vector3Int(0, 0, BaseZ);
	}

	private void UpdateTransform()
	{
		Vector3 vector = Blocks.Pivot(Coordinates, Orientation) + new Vector3(0f, 0f, BaseZ);
		if (FlipMode == FlipMode.Unflipped)
		{
			_positionModifier.Set(CoordinateSystem.GridToWorld(vector));
			_scaleModifier.Reset();
		}
		else
		{
			Vector3 coordinates = vector + Orientation.Transform(new Vector3Int(Blocks.Size.x, 0, 0));
			_positionModifier.Set(CoordinateSystem.GridToWorld(coordinates));
			_scaleModifier.Set(new Vector3(-1f, 1f, 1f));
		}
		_rotationModifier.Set(Orientation.ToWorldSpaceRotation());
	}
}
