using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.MechanicalSystem;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalNodeFacingMarkerDrawer : BaseComponent, IAwakableComponent, IUpdatableComponent, IPreviewSelectionListener, IPostPlacementChangeListener
{
	private static readonly Color MarkerColor = new Color(0.3f, 0.5f, 0.8f, 0.75f);

	private static readonly int MaxDistance = 20;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private readonly IBlockService _blockService;

	private readonly MarkerMatrix4x4Calculator _markerMatrix4X4Calculator;

	private MeshDrawer _meshDrawer;

	private BlockObject _blockObject;

	private MechanicalNode _mechanicalNode;

	private TransputProviderSpec _transputProviderSpec;

	private Preview _preview;

	private readonly List<Matrix4x4> _markers = new List<Matrix4x4>();

	private readonly List<BlockObject> _blockObjectCache = new List<BlockObject>();

	public MechanicalNodeFacingMarkerDrawer(MarkerDrawerFactory markerDrawerFactory, IBlockService blockService, MarkerMatrix4x4Calculator markerMatrix4X4Calculator)
	{
		_markerDrawerFactory = markerDrawerFactory;
		_blockService = blockService;
		_markerMatrix4X4Calculator = markerMatrix4X4Calculator;
	}

	public void Awake()
	{
		DisableComponent();
		_blockObject = GetComponent<BlockObject>();
		_mechanicalNode = GetComponent<MechanicalNode>();
		_transputProviderSpec = GetComponent<TransputProviderSpec>();
		_preview = GetComponent<Preview>();
		_meshDrawer = _markerDrawerFactory.CreateMechanicalInputMarkerDrawer(MarkerColor);
	}

	public void Update()
	{
		_meshDrawer.DrawMultiple(_markers);
	}

	public void OnPreviewSelect()
	{
		if (ShouldEnable(_preview.PreviewState.IsSingle))
		{
			EnableComponent();
			FindFacingTransputs();
		}
		else
		{
			DisableComponent();
		}
	}

	public void OnPreviewUnselect()
	{
		DisableComponent();
		_markers.Clear();
	}

	public void OnPostPlacementChanged()
	{
		if (base.Enabled)
		{
			FindFacingTransputs();
		}
	}

	private bool ShouldEnable(bool isSingleShownPreview)
	{
		if (isSingleShownPreview)
		{
			if (!_mechanicalNode.IsGenerator && !_mechanicalNode.IsShaft)
			{
				return _mechanicalNode.IsIntermediary;
			}
			return true;
		}
		return false;
	}

	private void FindFacingTransputs()
	{
		_markers.Clear();
		foreach (TransputSpec transput in _transputProviderSpec.Transputs)
		{
			FindFacingTransput(transput);
		}
	}

	private void FindFacingTransput(TransputSpec transputSpec)
	{
		foreach (Direction3D item in transputSpec.Directions.GetEnumerator())
		{
			Transput transput = new Transput(null, transputSpec, item, _blockObject);
			BlockOccupations blockOccupations = BlockOccupations.Bottom | BlockOccupations.Middle;
			if (transput.Direction == Direction3D.Bottom)
			{
				blockOccupations |= BlockOccupations.Top;
			}
			Vector3Int vector3Int = transput.Direction.ToOffset();
			for (int i = 1; i <= MaxDistance; i++)
			{
				Vector3Int vector3Int2 = vector3Int * i;
				Vector3Int coordinates = transput.Coordinates + vector3Int2;
				if (HasConnectableOccupation(coordinates, blockOccupations))
				{
					AddMarker(coordinates, transput);
					break;
				}
			}
		}
	}

	private bool HasConnectableOccupation(Vector3Int coordinates, BlockOccupations occupations)
	{
		_blockService.GetIntersectingObjectsAt(coordinates, occupations, _blockObjectCache);
		bool result = HasNonOverridableBlockObject();
		_blockObjectCache.Clear();
		return result;
	}

	private bool HasNonOverridableBlockObject()
	{
		foreach (BlockObject item in _blockObjectCache)
		{
			if (!item.Overridable)
			{
				return true;
			}
		}
		return false;
	}

	private void AddMarker(Vector3Int coordinates, Transput transput)
	{
		Transput transput2 = GetTransput(_blockService.GetBottomObjectComponentAt<MechanicalNode>(coordinates), coordinates, transput);
		if (transput2 != null)
		{
			_markers.Add(_markerMatrix4X4Calculator.CalculateMatrixFrom(transput2));
		}
	}

	private static Transput GetTransput(MechanicalNode otherNode, Vector3Int coordinates, Transput transput)
	{
		if ((bool)otherNode && !otherNode.IsShaft)
		{
			return otherNode.Transputs.SingleOrDefault((Transput otherTransput) => otherTransput.Coordinates == coordinates && otherTransput.Direction.Across() == transput.Direction);
		}
		return null;
	}
}
