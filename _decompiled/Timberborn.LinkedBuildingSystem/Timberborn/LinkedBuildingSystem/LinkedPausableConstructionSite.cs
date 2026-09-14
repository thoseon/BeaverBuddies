using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;

namespace Timberborn.LinkedBuildingSystem;

internal class LinkedPausableConstructionSite : BaseComponent, IAwakableComponent, IUnfinishedStateListener
{
	private PausableBuilding _pausableBuilding;

	private LinkedPausableConstructionSite _linked;

	private readonly MirrorOperationLock _mirrorOperationLock = new MirrorOperationLock();

	public void Awake()
	{
		_pausableBuilding = GetComponent<PausableBuilding>();
		GetComponent<LinkedBuilding>().BuildingLinked += OnBuildingLinked;
	}

	public void OnEnterUnfinishedState()
	{
		_pausableBuilding.PausedChanged += OnPausedChanged;
	}

	public void OnExitUnfinishedState()
	{
		_pausableBuilding.PausedChanged -= OnPausedChanged;
	}

	private void OnBuildingLinked(object sender, LinkedBuilding e)
	{
		_linked = e.GetComponent<LinkedPausableConstructionSite>();
	}

	private void OnPausedChanged(object sender, EventArgs e)
	{
		if (_mirrorOperationLock.IsUnlocked)
		{
			using (_mirrorOperationLock.Lock())
			{
				_linked.MirrorPauseState();
			}
		}
	}

	private void MirrorPauseState()
	{
		if (_linked._pausableBuilding.Paused)
		{
			_pausableBuilding.Pause();
		}
		else
		{
			_pausableBuilding.Resume();
		}
	}
}
