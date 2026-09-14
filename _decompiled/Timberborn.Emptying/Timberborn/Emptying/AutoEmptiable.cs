using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;

namespace Timberborn.Emptying;

public class AutoEmptiable : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private BlockableObject _blockableObject;

	private Emptiable _emptiable;

	private AutoEmptiableBlocker _autoEmptiableBlocker;

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_emptiable = GetComponent<Emptiable>();
		_autoEmptiableBlocker = GetComponent<AutoEmptiableBlocker>();
	}

	public void OnEnterFinishedState()
	{
		_blockableObject.ObjectUnblocked += OnObjectUnblocked;
		_blockableObject.ObjectBlocked += OnObjectBlocked;
		_autoEmptiableBlocker.BlockingStatusChanged += OnBlockingStatusChanged;
		UpdateEmptying();
	}

	public void OnExitFinishedState()
	{
		_blockableObject.ObjectUnblocked -= OnObjectUnblocked;
		_blockableObject.ObjectBlocked -= OnObjectBlocked;
		_autoEmptiableBlocker.BlockingStatusChanged -= OnBlockingStatusChanged;
		_emptiable.UnmarkForEmptying();
	}

	private void OnObjectUnblocked(object sender, EventArgs e)
	{
		UpdateEmptying();
	}

	private void OnObjectBlocked(object sender, EventArgs e)
	{
		UpdateEmptying();
	}

	private void OnBlockingStatusChanged(object sender, EventArgs e)
	{
		UpdateEmptying();
	}

	private void UpdateEmptying()
	{
		bool flag = !_blockableObject.IsUnblocked && !_autoEmptiableBlocker.IsBlocked;
		if (flag && !_emptiable.IsMarkedForEmptying)
		{
			_emptiable.MarkForEmptyingWithoutStatus();
		}
		else if (!flag && _emptiable.IsMarkedForEmptying)
		{
			_emptiable.UnmarkForEmptying();
		}
	}
}
