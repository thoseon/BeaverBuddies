using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.EnterableSystem;

public class Enterable : BaseComponent, IAwakableComponent, IInitializableEntity, IUnfinishedStateListener, IFinishedStateListener, IRegisteredComponent
{
	private BlockObject _blockObject;

	private readonly HashSet<Enterer> _enterersInside = new HashSet<Enterer>();

	private int _numberOfIncomingVisitors;

	public EnterableSpec EnterableSpec { get; private set; }

	public int Capacity
	{
		get
		{
			if (!_blockObject.IsFinished)
			{
				return EnterableSpec.CapacityUnfinished;
			}
			return EnterableSpec.CapacityFinished;
		}
	}

	public int NumberOfEnterersInside => _enterersInside.Count;

	public bool CanReserveSlot
	{
		get
		{
			if (base.Enabled)
			{
				if (LimitedCapacity)
				{
					return NumberOfReservedSlots < Capacity;
				}
				return true;
			}
			return false;
		}
	}

	public bool CanEnter
	{
		get
		{
			if (base.Enabled)
			{
				if (LimitedCapacity)
				{
					return _enterersInside.Count < Capacity;
				}
				return true;
			}
			return false;
		}
	}

	public Quaternion ExitWorldSpaceRotation => _blockObject.PositionedEntrance.Direction2D.Across().ToWorldSpaceRotation();

	public IEnumerable<Enterer> EnterersInside => _enterersInside.AsReadOnlyEnumerable();

	private bool LimitedCapacity
	{
		get
		{
			if (!_blockObject.IsFinished)
			{
				return EnterableSpec.LimitedCapacityUnfinished;
			}
			return EnterableSpec.LimitedCapacityFinished;
		}
	}

	private bool ShouldOperate
	{
		get
		{
			if (EnterableSpec.OperatingState != OperatingState.FinishedAndUnfinished && (!_blockObject.IsFinished || EnterableSpec.OperatingState != OperatingState.Finished))
			{
				if (!_blockObject.IsFinished)
				{
					return EnterableSpec.OperatingState == OperatingState.Unfinished;
				}
				return false;
			}
			return true;
		}
	}

	private int NumberOfReservedSlots => _numberOfIncomingVisitors + _enterersInside.Count;

	public event EventHandler<EntererAddedEventArgs> EntererAdded;

	public event EventHandler<EntererRemovedEventArgs> EntererRemoved;

	public void Awake()
	{
		EnterableSpec = GetComponent<EnterableSpec>();
		_blockObject = GetComponent<BlockObject>();
		DisableComponent();
	}

	public void InitializeEntity()
	{
		if (ShouldOperate)
		{
			EnableComponent();
		}
		else
		{
			DisableComponent();
		}
	}

	public void OnEnterUnfinishedState()
	{
		if (ShouldOperate)
		{
			EnableComponent();
		}
	}

	public void OnExitUnfinishedState()
	{
		if (ShouldOperate)
		{
			ForceRemoveEveryone();
			DisableComponent();
		}
	}

	public void OnEnterFinishedState()
	{
		if (ShouldOperate)
		{
			EnableComponent();
		}
	}

	public void OnExitFinishedState()
	{
		if (ShouldOperate)
		{
			ForceRemoveEveryone();
			DisableComponent();
		}
	}

	public void Add(Enterer enterer)
	{
		if (!CanEnter)
		{
			throw new InvalidOperationException($"Can't add enterer {enterer} to {base.Name}.");
		}
		_enterersInside.Add(enterer);
		EntererAdded?.Invoke(this, new EntererAddedEventArgs(enterer));
	}

	public void Remove(Enterer enterer)
	{
		if (!_enterersInside.Contains(enterer))
		{
			throw new ArgumentException($"Can't remove enterer {enterer} from {base.Name} " + "because it's not inside.");
		}
		_enterersInside.Remove(enterer);
		EntererRemoved?.Invoke(this, new EntererRemovedEventArgs(enterer));
	}

	public void ReserveSlot()
	{
		_numberOfIncomingVisitors++;
		ValidateReservedSlots();
	}

	public void UnreserveSlot()
	{
		_numberOfIncomingVisitors--;
		ValidateReservedSlots();
	}

	private void ForceRemoveEveryone()
	{
		Enterer[] array = _enterersInside.ToArray();
		foreach (Enterer enterer in array)
		{
			enterer.Abandon();
			Remove(enterer);
		}
	}

	private void ValidateReservedSlots()
	{
		if (LimitedCapacity && (NumberOfReservedSlots < 0 || NumberOfReservedSlots > Capacity))
		{
			Debug.LogError($"Reserved slots ({NumberOfReservedSlots}) of {base.Name} " + $"are out of bounds (0 to {Capacity})!");
		}
	}
}
