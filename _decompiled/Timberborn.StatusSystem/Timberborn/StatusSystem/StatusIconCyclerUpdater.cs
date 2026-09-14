using System.Collections.Generic;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

public class StatusIconCyclerUpdater : IUpdatableSingleton
{
	private static readonly float UpdateInterval = 1.25f;

	private readonly List<StatusIconCycler> _statusIconCyclers = new List<StatusIconCycler>();

	private float _nextUpdateTime;

	public void UpdateSingleton()
	{
		for (int num = _statusIconCyclers.Count - 1; num >= 0; num--)
		{
			_statusIconCyclers[num].UpdateStatusVisibility();
		}
		float unscaledTime = Time.unscaledTime;
		if (unscaledTime > _nextUpdateTime)
		{
			for (int num2 = _statusIconCyclers.Count - 1; num2 >= 0; num2--)
			{
				_statusIconCyclers[num2].IntervalUpdate();
			}
			_nextUpdateTime = unscaledTime + UpdateInterval;
		}
	}

	public void AddStatusIconCycler(StatusIconCycler statusIconCycler)
	{
		_statusIconCyclers.Add(statusIconCycler);
	}

	public void RemoveStatusIconCycler(StatusIconCycler statusIconCycler)
	{
		_statusIconCyclers.Remove(statusIconCycler);
	}
}
