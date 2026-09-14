using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;

namespace Timberborn.UnderstructureSystem;

internal class Understructure : BaseComponent, IDeletableEntity, IFinishedStateListener
{
	public event EventHandler Deleted;

	public event EventHandler EnteredFinishedState;

	public void DeleteEntity()
	{
		Deleted?.Invoke(this, EventArgs.Empty);
	}

	public void OnEnterFinishedState()
	{
		EnteredFinishedState?.Invoke(this, EventArgs.Empty);
	}

	public void OnExitFinishedState()
	{
	}
}
