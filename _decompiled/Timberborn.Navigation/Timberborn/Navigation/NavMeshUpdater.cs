using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal class NavMeshUpdater : ILoadableSingleton
{
	private readonly TerrainNavMeshSource _terrainNavMeshSource;

	private readonly PreviewTerrainNavMeshSource _previewTerrainNavMeshSource;

	private readonly InstantTerrainNavMeshSource _instantTerrainNavMeshSource;

	private readonly RoadNavMeshSource _roadNavMeshSource;

	private readonly PreviewRoadNavMeshSource _previewRoadNavMeshSource;

	private readonly InstantRoadNavMeshSource _instantRoadNavMeshSource;

	private readonly NavMeshChangeFactory _navMeshChangeFactory;

	private readonly NavMeshUpdateNotifier _navMeshUpdateNotifier;

	private readonly NavMeshUpdateBuilderFactory _navMeshUpdateBuilderFactory;

	private readonly Queue<NavMeshChange> _enqueuedRegularTerrainChanges = new Queue<NavMeshChange>();

	private readonly Queue<NavMeshChange> _enqueuedRegularRoadChanges = new Queue<NavMeshChange>();

	private readonly Queue<NavMeshChange> _enqueuedInstantTerrainChanges = new Queue<NavMeshChange>();

	private readonly Queue<NavMeshChange> _enqueuedInstantRoadChanges = new Queue<NavMeshChange>();

	private readonly Queue<NavMeshChange> _enqueuedPreviewTerrainChanges = new Queue<NavMeshChange>();

	private readonly Queue<NavMeshChange> _enqueuedPreviewRoadChanges = new Queue<NavMeshChange>();

	private NavMeshUpdate.Builder _previewNavMeshUpdateBuilder;

	public NavMeshUpdater(TerrainNavMeshSource terrainNavMeshSource, PreviewTerrainNavMeshSource previewTerrainNavMeshSource, InstantTerrainNavMeshSource instantTerrainNavMeshSource, RoadNavMeshSource roadNavMeshSource, PreviewRoadNavMeshSource previewRoadNavMeshSource, InstantRoadNavMeshSource instantRoadNavMeshSource, NavMeshChangeFactory navMeshChangeFactory, NavMeshUpdateNotifier navMeshUpdateNotifier, NavMeshUpdateBuilderFactory navMeshUpdateBuilderFactory)
	{
		_terrainNavMeshSource = terrainNavMeshSource;
		_previewTerrainNavMeshSource = previewTerrainNavMeshSource;
		_instantTerrainNavMeshSource = instantTerrainNavMeshSource;
		_roadNavMeshSource = roadNavMeshSource;
		_previewRoadNavMeshSource = previewRoadNavMeshSource;
		_instantRoadNavMeshSource = instantRoadNavMeshSource;
		_navMeshChangeFactory = navMeshChangeFactory;
		_navMeshUpdateNotifier = navMeshUpdateNotifier;
		_navMeshUpdateBuilderFactory = navMeshUpdateBuilderFactory;
	}

	public void Load()
	{
		_previewNavMeshUpdateBuilder = _navMeshUpdateBuilderFactory.Create();
	}

	public void EnqueueRegularChange(in NavMeshChangeSpecification specification)
	{
		NavMeshChange item = _navMeshChangeFactory.Create(in specification);
		_enqueuedRegularTerrainChanges.Enqueue(item);
		_enqueuedInstantTerrainChanges.Enqueue(item);
		bool flag = specification.NavMeshEdge.IsRoad;
		if (!flag)
		{
			NavMeshChangeType navMeshChangeType = specification.NavMeshChangeType;
			bool flag2 = ((navMeshChangeType == NavMeshChangeType.BlockEdge || navMeshChangeType == NavMeshChangeType.UnblockEdge) ? true : false);
			flag = flag2;
		}
		if (flag)
		{
			_enqueuedRegularRoadChanges.Enqueue(item);
			_enqueuedInstantRoadChanges.Enqueue(item);
		}
	}

	public void EnqueueRegularChanges(IReadOnlyList<NavMeshChangeSpecification> specifications)
	{
		for (int i = 0; i < specifications.Count; i++)
		{
			EnqueueRegularChange(specifications[i]);
		}
	}

	public void EnqueuePreviewChange(in NavMeshChangeSpecification specification)
	{
		NavMeshChange item = _navMeshChangeFactory.Create(in specification);
		_enqueuedPreviewTerrainChanges.Enqueue(item);
		bool flag = specification.NavMeshEdge.IsRoad;
		if (!flag)
		{
			NavMeshChangeType navMeshChangeType = specification.NavMeshChangeType;
			bool flag2 = ((navMeshChangeType == NavMeshChangeType.BlockEdge || navMeshChangeType == NavMeshChangeType.UnblockEdge) ? true : false);
			flag = flag2;
		}
		if (flag)
		{
			_enqueuedPreviewRoadChanges.Enqueue(item);
		}
	}

	public void EnqueuePreviewChanges(IReadOnlyList<NavMeshChangeSpecification> specifications)
	{
		for (int i = 0; i < specifications.Count; i++)
		{
			EnqueuePreviewChange(specifications[i]);
		}
	}

	public void ApplyPreviewChanges(IReadOnlyList<NavMeshChangeSpecification> specifications)
	{
		_previewNavMeshUpdateBuilder.Reset();
		for (int i = 0; i < specifications.Count; i++)
		{
			NavMeshChangeSpecification navMeshChangeSpecification = specifications[i];
			NavMeshChange navMeshChange = _navMeshChangeFactory.Create(in navMeshChangeSpecification);
			navMeshChange.Apply(_previewTerrainNavMeshSource, _previewNavMeshUpdateBuilder);
			NavMeshChangeType navMeshChangeType = navMeshChangeSpecification.NavMeshChangeType;
			bool flag = ((navMeshChangeType == NavMeshChangeType.BlockEdge || navMeshChangeType == NavMeshChangeType.UnblockEdge) ? true : false);
			bool flag2 = flag;
			if (!flag2)
			{
				bool flag3 = navMeshChangeSpecification.NavMeshEdge.IsRoad;
				if (flag3)
				{
					NavMeshChangeType navMeshChangeType2 = navMeshChangeSpecification.NavMeshChangeType;
					bool flag4 = ((navMeshChangeType2 == NavMeshChangeType.AddEdge || navMeshChangeType2 == NavMeshChangeType.RemoveEdge) ? true : false);
					flag3 = flag4;
				}
				flag2 = flag3;
			}
			if (flag2)
			{
				navMeshChange.Apply(_previewRoadNavMeshSource, _previewNavMeshUpdateBuilder);
			}
		}
		if (!_previewNavMeshUpdateBuilder.IsEmpty)
		{
			_navMeshUpdateNotifier.NotifyOfPreviewNavMeshUpdates(_previewNavMeshUpdateBuilder.Build());
		}
	}

	public void ProcessRegularChanges(NavMeshUpdate.Builder navMeshUpdateBuilder)
	{
		if (!_enqueuedRegularTerrainChanges.IsEmpty())
		{
			while (!_enqueuedRegularTerrainChanges.IsEmpty())
			{
				_enqueuedRegularTerrainChanges.Dequeue().Apply(_terrainNavMeshSource, navMeshUpdateBuilder);
			}
			while (!_enqueuedRegularRoadChanges.IsEmpty())
			{
				_enqueuedRegularRoadChanges.Dequeue().Apply(_roadNavMeshSource, navMeshUpdateBuilder);
			}
		}
	}

	public void ProcessPreviewChanges(NavMeshUpdate.Builder navMeshUpdateBuilder)
	{
		if (!_enqueuedPreviewTerrainChanges.IsEmpty())
		{
			while (!_enqueuedPreviewTerrainChanges.IsEmpty())
			{
				_enqueuedPreviewTerrainChanges.Dequeue().Apply(_previewTerrainNavMeshSource, navMeshUpdateBuilder);
			}
			while (!_enqueuedPreviewRoadChanges.IsEmpty())
			{
				_enqueuedPreviewRoadChanges.Dequeue().Apply(_previewRoadNavMeshSource, navMeshUpdateBuilder);
			}
		}
	}

	public void ProcessInstantChanges(NavMeshUpdate.Builder navMeshUpdateBuilder)
	{
		if (!_enqueuedInstantTerrainChanges.IsEmpty())
		{
			while (!_enqueuedInstantTerrainChanges.IsEmpty())
			{
				NavMeshChange navMeshChange = _enqueuedInstantTerrainChanges.Dequeue();
				navMeshChange.Apply(_instantTerrainNavMeshSource, navMeshUpdateBuilder);
				navMeshChange.Apply(_previewTerrainNavMeshSource, navMeshUpdateBuilder);
			}
			while (!_enqueuedInstantRoadChanges.IsEmpty())
			{
				NavMeshChange navMeshChange2 = _enqueuedInstantRoadChanges.Dequeue();
				navMeshChange2.Apply(_instantRoadNavMeshSource, navMeshUpdateBuilder);
				navMeshChange2.Apply(_previewRoadNavMeshSource, navMeshUpdateBuilder);
			}
		}
	}

	public void TrimExcess()
	{
		_enqueuedRegularTerrainChanges.TrimExcess();
		_enqueuedRegularRoadChanges.TrimExcess();
		_enqueuedInstantTerrainChanges.TrimExcess();
		_enqueuedInstantRoadChanges.TrimExcess();
		_enqueuedPreviewTerrainChanges.TrimExcess();
		_enqueuedPreviewRoadChanges.TrimExcess();
	}
}
