using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.MapEditorTickSystem;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.SoilBarrierSystem;

[MapEditorTickable]
public class SoilBarrierMap : ILoadableSingleton, ITickableSingleton
{
	private readonly struct Modification
	{
		public bool Added { get; }

		public int Index { get; }

		public BarrierType BarrierType { get; }

		public Modification(bool added, int index, BarrierType barrierType)
		{
			Added = added;
			Index = index;
			BarrierType = barrierType;
		}
	}

	private enum BarrierType
	{
		AboveMoisture,
		FullMoisture,
		Contamination
	}

	private readonly MapIndexService _mapIndexService;

	private readonly Queue<Modification> _modifications = new Queue<Modification>();

	private bool[] _aboveMoistureBarriers;

	private bool[] _fullMoistureBarriers;

	private bool[] _contaminationBarriers;

	public ReadOnlyArray<bool> AboveMoistureBarriers => new ReadOnlyArray<bool>(_aboveMoistureBarriers);

	public ReadOnlyArray<bool> FullMoistureBarriers => new ReadOnlyArray<bool>(_fullMoistureBarriers);

	public ReadOnlyArray<bool> ContaminationBarriers => new ReadOnlyArray<bool>(_contaminationBarriers);

	public SoilBarrierMap(MapIndexService mapIndexService)
	{
		_mapIndexService = mapIndexService;
	}

	public void Load()
	{
		int maxSize3D = _mapIndexService.MaxSize3D;
		_aboveMoistureBarriers = new bool[maxSize3D];
		_fullMoistureBarriers = new bool[maxSize3D];
		_contaminationBarriers = new bool[maxSize3D];
	}

	public void AddAboveMoistureBarrierAt(Vector3Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: true, GetIndex(coordinates), BarrierType.AboveMoisture));
	}

	public void RemoveAboveMoistureBarrierAt(Vector3Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: false, GetIndex(coordinates), BarrierType.AboveMoisture));
	}

	public void AddFullMoistureBarrierAt(Vector3Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: true, GetIndex(coordinates), BarrierType.FullMoisture));
	}

	public void RemoveFullMoistureBarrierAt(Vector3Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: false, GetIndex(coordinates), BarrierType.FullMoisture));
	}

	public void AddContaminationBarrierAt(Vector3Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: true, GetIndex(coordinates), BarrierType.Contamination));
	}

	public void RemoveContaminationBarrierAt(Vector3Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: false, GetIndex(coordinates), BarrierType.Contamination));
	}

	public void Tick()
	{
		ProcessModifications();
	}

	private void ProcessModifications()
	{
		while (!_modifications.IsEmpty())
		{
			Modification modification = _modifications.Dequeue();
			switch (modification.BarrierType)
			{
			case BarrierType.AboveMoisture:
				_aboveMoistureBarriers[modification.Index] = modification.Added;
				break;
			case BarrierType.FullMoisture:
				_fullMoistureBarriers[modification.Index] = modification.Added;
				break;
			case BarrierType.Contamination:
				_contaminationBarriers[modification.Index] = modification.Added;
				break;
			}
		}
	}

	private int GetIndex(Vector3Int coordinates)
	{
		return _mapIndexService.CoordinatesToIndex3D(coordinates);
	}
}
