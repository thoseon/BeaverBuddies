using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.ErrorReporting;
using Timberborn.MapIndexSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainPhysics;

public class TerrainPhysicsPostLoader
{
	private readonly struct Candidate
	{
		public Vector3Int Coordinates { get; }

		public byte Distance { get; }

		public Candidate(Vector3Int coordinates, int distance)
		{
			Coordinates = coordinates;
			Distance = (byte)distance;
		}
	}

	private static readonly string TerrainHasNoSupportLocKey = "TerrainPhysicsPostLoader.TerrainHasNoSupport";

	private static readonly string BlockObjectLoadingIssueLocKey = "LoadingIssue.BlockObjectLoadingIssue";

	private readonly ITerrainService _terrainService;

	private readonly MapIndexService _mapIndexService;

	private readonly EntityRegistry _entityRegistry;

	private readonly EntityService _entityService;

	private readonly IBlockService _blockService;

	private readonly MatterBelowValidator _matterBelowValidator;

	private readonly TerrainPhysicsValidationEnabler _terrainPhysicsValidationEnabler;

	private readonly ILoadingIssueService _loadingIssueService;

	private HashSet<BlockObject> _validBlockObjects;

	private HashSet<Vector3Int> _validTerrain;

	private Queue<Candidate> _candidates;

	private byte[] _visited;

	public TerrainPhysicsPostLoader(ITerrainService terrainService, MapIndexService mapIndexService, EntityRegistry entityRegistry, EntityService entityService, IBlockService blockService, MatterBelowValidator matterBelowValidator, TerrainPhysicsValidationEnabler terrainPhysicsValidationEnabler, ILoadingIssueService loadingIssueService)
	{
		_terrainService = terrainService;
		_mapIndexService = mapIndexService;
		_entityRegistry = entityRegistry;
		_entityService = entityService;
		_blockService = blockService;
		_matterBelowValidator = matterBelowValidator;
		_terrainPhysicsValidationEnabler = terrainPhysicsValidationEnabler;
		_loadingIssueService = loadingIssueService;
	}

	public void ValidateAll()
	{
		bool flag = true;
		while (flag)
		{
			flag = Validate();
		}
		_terrainPhysicsValidationEnabler.Enable();
	}

	private bool Validate()
	{
		CreateCollections();
		GetInitialCandidates();
		while (_candidates.Count > 0)
		{
			ValidateCandidate();
		}
		bool result = RemoveBlockObjects() || RemoveTerrain();
		ClearCollections();
		return result;
	}

	private void CreateCollections()
	{
		_validBlockObjects = new HashSet<BlockObject>();
		_validTerrain = new HashSet<Vector3Int>();
		_candidates = new Queue<Candidate>();
		_visited = CreateVisitedArray();
	}

	private byte[] CreateVisitedArray()
	{
		byte[] array = new byte[_mapIndexService.VerticalStride * _mapIndexService.TotalSize.z];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = byte.MaxValue;
		}
		return array;
	}

	private void GetInitialCandidates()
	{
		for (int i = 0; i < _mapIndexService.TerrainSize.y; i++)
		{
			for (int j = 0; j < _mapIndexService.TerrainSize.x; j++)
			{
				_candidates.Enqueue(new Candidate(new Vector3Int(j, i, 0), 0));
			}
		}
	}

	private void ValidateCandidate()
	{
		Candidate candidate = _candidates.Dequeue();
		Vector3Int coordinates = candidate.Coordinates;
		int num = _mapIndexService.CoordinatesToIndex3D(coordinates);
		if (candidate.Distance < _visited[num])
		{
			_visited[num] = candidate.Distance;
			ValidateTerrain(coordinates, candidate);
			ValidateBlockObjects(coordinates, candidate);
		}
	}

	private void ValidateTerrain(Vector3Int coordinates, Candidate candidate)
	{
		if (_terrainService.Underground(coordinates))
		{
			_validTerrain.Add(coordinates);
			Enqueue(coordinates.Above(), 0);
			if (candidate.Distance < TerrainPhysicsValidator.MaxSupportDistance)
			{
				EnqueueNeighbors(coordinates, candidate.Distance + 1);
			}
		}
	}

	private void ValidateBlockObjects(Vector3Int coordinates, Candidate candidate)
	{
		foreach (BlockObject item in _blockService.GetObjectsAt(coordinates))
		{
			if (IsValid(item))
			{
				_validBlockObjects.Add(item);
				foreach (Block allBlock in item.PositionedBlocks.GetAllBlocks())
				{
					Enqueue(candidate, allBlock);
				}
			}
		}
	}

	private bool IsValid(BlockObject blockObject)
	{
		foreach (Block foundationBlock in blockObject.PositionedBlocks.GetFoundationBlocks())
		{
			Block block = foundationBlock;
			if (!_matterBelowValidator.Validate(in block))
			{
				return false;
			}
		}
		return true;
	}

	private void Enqueue(Candidate candidate, Block block)
	{
		BlockStackable stackable = block.Stackable;
		if (stackable.IsStackable())
		{
			Enqueue(block.Coordinates.Above(), 0);
			if (stackable == BlockStackable.UnfinishedGround && candidate.Distance < TerrainPhysicsValidator.MaxSupportDistance)
			{
				EnqueueNeighbors(block.Coordinates, candidate.Distance + 1);
			}
		}
	}

	private void EnqueueNeighbors(Vector3Int coordinates, int distance)
	{
		Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
		foreach (Vector3Int vector3Int in neighbors4Vector3Int)
		{
			Enqueue(coordinates + vector3Int, distance);
		}
	}

	private void Enqueue(Vector3Int coordinates, int distance)
	{
		if (Sizing.SizeContains(_mapIndexService.TotalSize, coordinates))
		{
			_candidates.Enqueue(new Candidate(coordinates, distance));
		}
	}

	private bool RemoveTerrain()
	{
		bool result = false;
		foreach (Vector3Int item in GetTerrainToUnset())
		{
			_loadingIssueService.AddIssue($"Loaded terrain at {item}" + " is not supported by terrain physics. Deleting it.", TerrainHasNoSupportLocKey);
			_terrainService.UnsetTerrain(item);
			result = true;
		}
		return result;
	}

	private List<Vector3Int> GetTerrainToUnset()
	{
		List<Vector3Int> list = new List<Vector3Int>();
		for (int i = 0; i < _mapIndexService.TerrainSize.y; i++)
		{
			for (int j = 0; j < _mapIndexService.TerrainSize.x; j++)
			{
				int num = _mapIndexService.CellToIndex(new Vector2Int(j, i));
				int columnCount = _terrainService.GetColumnCount(num);
				for (int k = 1; k < columnCount; k++)
				{
					int index3D = num + k * _mapIndexService.VerticalStride;
					int columnFloor = _terrainService.GetColumnFloor(index3D);
					int columnCeiling = _terrainService.GetColumnCeiling(index3D);
					for (int l = columnFloor; l < columnCeiling; l++)
					{
						Vector3Int item = new Vector3Int(j, i, l);
						if (!_validTerrain.Contains(item))
						{
							list.Add(item);
						}
					}
				}
			}
		}
		return list;
	}

	private bool RemoveBlockObjects()
	{
		bool result = false;
		foreach (BlockObject item in (from entityComponent in _entityRegistry.Entities
			select entityComponent.GetComponent<BlockObject>() into blockObject
			where blockObject
			select blockObject).ToList())
		{
			if (!_validBlockObjects.Contains(item))
			{
				LabeledEntitySpec component = item.GetComponent<LabeledEntitySpec>();
				_loadingIssueService.AddIssue("Loaded BlockObject " + item.Name + " at " + $"{item.Coordinates} is not supported by terrain physics. Deleting it.", BlockObjectLoadingIssueLocKey, component.DisplayNameLocKey, paramIsLocKey: true);
				_entityService.Delete(item);
				result = true;
			}
		}
		return result;
	}

	private void ClearCollections()
	{
		_validBlockObjects = null;
		_validTerrain = null;
		_candidates = null;
		_visited = null;
	}
}
