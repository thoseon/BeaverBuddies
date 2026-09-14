using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Demolishing;
using Timberborn.NaturalResources;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.NaturalResourcesReproduction;

public class NaturalResourceReproducer : ITickableSingleton, ILoadableSingleton
{
	private readonly struct ReproducibleKey : IEquatable<ReproducibleKey>
	{
		public string Id { get; }

		public float ReproductionChance { get; }

		private ReproducibleKey(string id, float reproductionChance)
		{
			Id = id;
			ReproductionChance = reproductionChance;
		}

		public static ReproducibleKey Create(Reproducible reproducible)
		{
			return new ReproducibleKey(reproducible.Id, reproducible.ReproductionChance);
		}

		public bool Equals(ReproducibleKey other)
		{
			return Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			if (obj is ReproducibleKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (Id == null)
			{
				return 0;
			}
			return Id.GetHashCode();
		}
	}

	private readonly IBlockService _blockService;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly EventBus _eventBus;

	private readonly NaturalResourceFactory _naturalResourceFactory;

	private readonly Dictionary<ReproducibleKey, HashSet<Vector3Int>> _potentialSpots = new Dictionary<ReproducibleKey, HashSet<Vector3Int>>();

	private readonly List<(ReproducibleKey, Vector3Int)> _newResources = new List<(ReproducibleKey, Vector3Int)>();

	public IEnumerable<Vector3Int> PotentialSpots => _potentialSpots.SelectMany((KeyValuePair<ReproducibleKey, HashSet<Vector3Int>> pair) => pair.Value);

	public NaturalResourceReproducer(IBlockService blockService, IDayNightCycle dayNightCycle, IRandomNumberGenerator randomNumberGenerator, EventBus eventBus, NaturalResourceFactory naturalResourceFactory)
	{
		_blockService = blockService;
		_dayNightCycle = dayNightCycle;
		_randomNumberGenerator = randomNumberGenerator;
		_eventBus = eventBus;
		_naturalResourceFactory = naturalResourceFactory;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void Tick()
	{
		TryReproduceResources();
	}

	public void MarkSpots(Reproducible reproducible)
	{
		Vector3Int coordinates = reproducible.GetComponent<BlockObject>().Coordinates;
		ReproducibleKey key = ReproducibleKey.Create(reproducible);
		HashSet<Vector3Int> orAdd = _potentialSpots.GetOrAdd(key, () => new HashSet<Vector3Int>());
		Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
		foreach (Vector3Int vector3Int in neighbors4Vector3Int)
		{
			orAdd.Add(coordinates + vector3Int);
		}
	}

	public void UnmarkSpots(Reproducible reproducible)
	{
		ReproducibleKey key = ReproducibleKey.Create(reproducible);
		if (!_potentialSpots.TryGetValue(key, out var value))
		{
			return;
		}
		Vector3Int coordinates = reproducible.GetComponent<BlockObject>().Coordinates;
		Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
		foreach (Vector3Int vector3Int in neighbors4Vector3Int)
		{
			Vector3Int vector3Int2 = coordinates + vector3Int;
			if (value.Contains(vector3Int2) && !CanReproduceAtCoordinates(reproducible.Id, vector3Int2))
			{
				value.Remove(vector3Int2);
			}
		}
	}

	private void TryReproduceResources()
	{
		float num = _dayNightCycle.FixedDeltaTimeInHours / 24f;
		foreach (KeyValuePair<ReproducibleKey, HashSet<Vector3Int>> potentialSpot in _potentialSpots)
		{
			float num2 = num * potentialSpot.Key.ReproductionChance;
			float num3 = _randomNumberGenerator.Range(0f, 1f);
			HashSet<Vector3Int> value = potentialSpot.Value;
			if (num3 < num2 * (float)value.Count)
			{
				int index = _randomNumberGenerator.Range(0, value.Count);
				_newResources.Add((potentialSpot.Key, potentialSpot.Value.ElementAt(index)));
			}
		}
		SpawnNewResources();
	}

	private void SpawnNewResources()
	{
		foreach (var newResource in _newResources)
		{
			ReproducibleKey item = newResource.Item1;
			Vector3Int item2 = newResource.Item2;
			NaturalResource naturalResource = _naturalResourceFactory.SpawnNew(item.Id, item2);
			if ((bool)naturalResource && AnyNeighborMarkedForDemolish(item, item2))
			{
				naturalResource.GetComponent<Demolishable>().Mark();
			}
		}
		_newResources.Clear();
	}

	private bool CanReproduceAtCoordinates(string id, Vector3Int coordinates)
	{
		Vector2Int[] neighbors4Vector2Int = Deltas.Neighbors4Vector2Int;
		foreach (Vector2Int value in neighbors4Vector2Int)
		{
			Vector3Int coordinates2 = coordinates + value.XYZ();
			Reproducible bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<Reproducible>(coordinates2);
			if (bottomObjectComponentAt != null && bottomObjectComponentAt.Id == id && !bottomObjectComponentAt.ReproductionDisabled && bottomObjectComponentAt.GetComponent<BlockObject>().Coordinates.z == coordinates.z)
			{
				return true;
			}
		}
		return false;
	}

	private bool AnyNeighborMarkedForDemolish(ReproducibleKey reproducibleKey, Vector3Int coordinates)
	{
		Vector2Int[] neighbors4Vector2Int = Deltas.Neighbors4Vector2Int;
		foreach (Vector2Int value in neighbors4Vector2Int)
		{
			Vector3Int coordinates2 = coordinates + value.XYZ();
			NaturalResource bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<NaturalResource>(coordinates2);
			if ((bool)bottomObjectComponentAt && bottomObjectComponentAt.GetComponent<TemplateSpec>().TemplateName == reproducibleKey.Id)
			{
				Demolishable component = bottomObjectComponentAt.GetComponent<Demolishable>();
				if (component != null && component.IsMarked)
				{
					return true;
				}
			}
		}
		return false;
	}
}
