using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockSystemUI;
using Timberborn.BlueprintSystem;
using Timberborn.BuildingRange;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.EnterableSystem;

public class RangeEnterableHighlighter : BaseComponent, IAwakableComponent, IUpdatableComponent, ISelectionListener, IPreviewSelectionListener, IPostPlacementChangeListener
{
	private readonly BlockObjectBoundsDrawerFactory _blockObjectBoundsDrawerFactory;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly ISpecService _specService;

	private IBuildingWithRange _buildingWithRange;

	private BlockObjectBoundsDrawer _blockObjectBoundsDrawer;

	private readonly HashSet<Vector2Int> _blocks = new HashSet<Vector2Int>();

	public RangeEnterableHighlighter(BlockObjectBoundsDrawerFactory blockObjectBoundsDrawerFactory, EntityComponentRegistry entityComponentRegistry, ISpecService specService)
	{
		_blockObjectBoundsDrawerFactory = blockObjectBoundsDrawerFactory;
		_entityComponentRegistry = entityComponentRegistry;
		_specService = specService;
	}

	public void Awake()
	{
		_buildingWithRange = GetComponent<IBuildingWithRange>();
		RangeEnterableHighlighterSpec singleSpec = _specService.GetSingleSpec<RangeEnterableHighlighterSpec>();
		_blockObjectBoundsDrawer = _blockObjectBoundsDrawerFactory.Create(singleSpec.BuildingInRange);
		DisableComponent();
	}

	public void Update()
	{
		HighlightBuildings();
	}

	public void OnSelect()
	{
		RecalculateBlocks();
		EnableComponent();
	}

	public void OnUnselect()
	{
		_blocks.Clear();
		DisableComponent();
	}

	public void OnPreviewSelect()
	{
		HighlightBuildings();
	}

	public void OnPreviewUnselect()
	{
		OnUnselect();
	}

	public void OnPostPlacementChanged()
	{
		RecalculateBlocks();
	}

	private void RecalculateBlocks()
	{
		_blocks.Clear();
		_blocks.AddRange(_buildingWithRange.GetBlocksInRange().XY());
	}

	private void HighlightBuildings()
	{
		foreach (Enterable item in _entityComponentRegistry.GetEnabled<Enterable>())
		{
			BlockObject component = item.GetComponent<BlockObject>();
			if (component.PositionedBlocks.GetAllCoordinates().Any((Vector3Int coordinate) => _blocks.Contains(coordinate.XY())))
			{
				_blockObjectBoundsDrawer.DrawBounds(component);
			}
		}
	}
}
