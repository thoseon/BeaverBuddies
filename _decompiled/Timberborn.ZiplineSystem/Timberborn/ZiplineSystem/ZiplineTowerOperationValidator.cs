using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.ZiplineSystem;

public class ZiplineTowerOperationValidator : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private static readonly float MinConnectionsForStation = 1f;

	private static readonly float MinConnectionsForPylon = 2f;

	private readonly ZiplineCableRenderer _ziplineCableRenderer;

	private ZiplineTower _ziplineTower;

	private readonly HashSet<ZiplineTower> _connectedTowersCache = new HashSet<ZiplineTower>();

	private readonly Queue<ZiplineTower> _towersToCheckCache = new Queue<ZiplineTower>();

	public bool IsOperative { get; private set; }

	public event EventHandler OperativeStateChanged;

	public ZiplineTowerOperationValidator(ZiplineCableRenderer ziplineCableRenderer)
	{
		_ziplineCableRenderer = ziplineCableRenderer;
	}

	public void Awake()
	{
		_ziplineTower = GetComponent<ZiplineTower>();
	}

	public void OnEnterFinishedState()
	{
		_ziplineTower.ConnectionTargetsChanged += OnConnectionTargetsChanged;
		UpdateState();
		NotifyConnectedInNetwork();
	}

	public void OnExitFinishedState()
	{
		_ziplineTower.ConnectionTargetsChanged -= OnConnectionTargetsChanged;
	}

	private void OnConnectionTargetsChanged(object sender, EventArgs e)
	{
		UpdateState();
		NotifyConnectedInNetwork();
	}

	private void UpdateState()
	{
		FindConnectedTowers(_ziplineTower);
		int num = 0;
		foreach (ZiplineTower item in _connectedTowersCache)
		{
			if (item.HasComponent<ZiplineStationSpec>())
			{
				num++;
			}
		}
		ClearCache();
		UpdateState(num);
	}

	private void UpdateState(int connectedStations)
	{
		float num = (_ziplineTower.HasComponent<ZiplineStationSpec>() ? MinConnectionsForStation : MinConnectionsForPylon);
		bool flag = (float)connectedStations >= num && _ziplineTower.IsActive;
		if (flag != IsOperative)
		{
			IsOperative = flag;
			OperativeStateChanged?.Invoke(this, EventArgs.Empty);
			UpdateRenderer();
		}
	}

	private void NotifyConnectedInNetwork()
	{
		FindConnectedTowers(_ziplineTower);
		foreach (ZiplineTower item in _connectedTowersCache)
		{
			item.GetComponent<ZiplineTowerOperationValidator>().UpdateState();
		}
		ClearCache();
	}

	private void FindConnectedTowers(ZiplineTower startTower)
	{
		foreach (ZiplineTower connectionTarget in startTower.ConnectionTargets)
		{
			if (connectionTarget.IsActive)
			{
				_towersToCheckCache.Enqueue(connectionTarget);
			}
		}
		while (_towersToCheckCache.Count > 0)
		{
			ZiplineTower ziplineTower = _towersToCheckCache.Dequeue();
			if (ziplineTower == startTower || !_connectedTowersCache.Add(ziplineTower) || ziplineTower.HasComponent<ZiplineStationSpec>())
			{
				continue;
			}
			foreach (ZiplineTower connectionTarget2 in ziplineTower.ConnectionTargets)
			{
				if (connectionTarget2 != startTower && connectionTarget2.IsActive)
				{
					_towersToCheckCache.Enqueue(connectionTarget2);
				}
			}
		}
	}

	private void ClearCache()
	{
		_connectedTowersCache.Clear();
		_towersToCheckCache.Clear();
	}

	private void UpdateRenderer()
	{
		foreach (ZiplineTower connectionTarget in _ziplineTower.ConnectionTargets)
		{
			_ziplineCableRenderer.UpdateConnection(_ziplineTower, connectionTarget);
		}
	}
}
