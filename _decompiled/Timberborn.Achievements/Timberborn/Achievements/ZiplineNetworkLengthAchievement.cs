using System.Collections.Generic;
using Timberborn.AchievementSystem;
using Timberborn.SingletonSystem;
using Timberborn.ZiplineSystem;
using UnityEngine;

namespace Timberborn.Achievements;

internal class ZiplineNetworkLengthAchievement : Achievement
{
	private static readonly float MinimumLength = 1000f;

	private readonly EventBus _eventBus;

	private readonly Queue<ZiplineTower> _towersToVisit = new Queue<ZiplineTower>();

	private readonly HashSet<ZiplineTower> _visitedTowers = new HashSet<ZiplineTower>();

	private readonly HashSet<(ZiplineTower, ZiplineTower)> _visitedEdges = new HashSet<(ZiplineTower, ZiplineTower)>();

	public override string Id => "ZIPLINE_NETWORK_LENGTH";

	public ZiplineNetworkLengthAchievement(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	[OnEvent]
	public void OnZiplineConnectionActivated(ZiplineConnectionActivatedEvent ziplineConnectionActivatedEvent)
	{
		CheckLengthFrom(ziplineConnectionActivatedEvent.ZiplineTower);
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private void CheckLengthFrom(ZiplineTower startTower)
	{
		_towersToVisit.Enqueue(startTower);
		while (_towersToVisit.Count > 0)
		{
			ZiplineTower ziplineTower = _towersToVisit.Dequeue();
			if (!_visitedTowers.Add(ziplineTower))
			{
				continue;
			}
			foreach (ZiplineTower connectionTarget in ziplineTower.ConnectionTargets)
			{
				_visitedEdges.Add((connectionTarget, ziplineTower));
				if (!_visitedTowers.Contains(connectionTarget))
				{
					_towersToVisit.Enqueue(connectionTarget);
				}
			}
		}
		float num = 0f;
		foreach (var visitedEdge in _visitedEdges)
		{
			ZiplineTower item = visitedEdge.Item1;
			ZiplineTower item2 = visitedEdge.Item2;
			num += Vector3.Distance(item.CableAnchorPoint, item2.CableAnchorPoint);
		}
		_visitedTowers.Clear();
		_towersToVisit.Clear();
		_visitedEdges.Clear();
		if (num >= MinimumLength)
		{
			Unlock();
		}
	}
}
