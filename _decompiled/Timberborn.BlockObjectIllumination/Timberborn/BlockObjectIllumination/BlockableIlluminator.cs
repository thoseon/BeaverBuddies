using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Illumination;

namespace Timberborn.BlockObjectIllumination;

internal class BlockableIlluminator : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private IlluminatorToggle _illuminatorToggle;

	private BlockableObject _blockableObject;

	public void Awake()
	{
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
		_blockableObject = GetComponent<BlockableObject>();
	}

	public void OnEnterFinishedState()
	{
		_blockableObject.ObjectUnblocked += OnObjectUnblocked;
		_blockableObject.ObjectBlocked += OnObjectBlocked;
		if (_blockableObject.IsUnblocked)
		{
			_illuminatorToggle.TurnOn();
		}
	}

	public void OnExitFinishedState()
	{
		_blockableObject.ObjectUnblocked -= OnObjectUnblocked;
		_blockableObject.ObjectBlocked -= OnObjectBlocked;
		_illuminatorToggle.TurnOff();
	}

	private void OnObjectUnblocked(object sender, EventArgs e)
	{
		_illuminatorToggle.TurnOn();
	}

	private void OnObjectBlocked(object sender, EventArgs e)
	{
		_illuminatorToggle.TurnOff();
	}
}
