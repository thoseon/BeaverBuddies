using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Bots;
using Timberborn.EntitySystem;

namespace Timberborn.TubeSystem;

internal class Tube : BaseComponent, IAwakableComponent, IFinishedStateListener, IDeletableEntity
{
	private readonly TubeMap _tubeMap;

	private BlockObject _blockObject;

	private readonly HashSet<TubeVisitor> _beaverVisitors = new HashSet<TubeVisitor>();

	private readonly HashSet<TubeVisitor> _botVisitors = new HashSet<TubeVisitor>();

	public bool HasAnyVisitor
	{
		get
		{
			if (_beaverVisitors.Count <= 0)
			{
				return _botVisitors.Count > 0;
			}
			return true;
		}
	}

	public bool HasBotVisitor => _botVisitors.Count > 0;

	public bool CanBeVisited => _blockObject.IsFinished;

	public event EventHandler TubeDeleted;

	public event EventHandler VisitorsChanged;

	public Tube(TubeMap tubeMap)
	{
		_tubeMap = tubeMap;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void OnEnterFinishedState()
	{
		_tubeMap.SetTube(this, _blockObject.Coordinates);
	}

	public void OnExitFinishedState()
	{
		_tubeMap.UnsetTube(_blockObject.Coordinates);
	}

	public void AddVisitor(TubeVisitor visitor)
	{
		if (GetVisitors(visitor).Add(visitor))
		{
			VisitorsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void RemoveVisitor(TubeVisitor visitor)
	{
		if (GetVisitors(visitor).Remove(visitor))
		{
			VisitorsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void DeleteEntity()
	{
		TubeDeleted?.Invoke(this, EventArgs.Empty);
	}

	private HashSet<TubeVisitor> GetVisitors(TubeVisitor visitor)
	{
		if (!visitor.HasComponent<BotSpec>())
		{
			return _beaverVisitors;
		}
		return _botVisitors;
	}
}
