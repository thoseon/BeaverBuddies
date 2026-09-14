using System;
using Timberborn.BaseComponentSystem;

namespace Timberborn.Carrying;

public class BackpackCarrier : BaseComponent
{
	public bool IsBackpackEnabled { get; private set; }

	public event EventHandler BackpackChanged;

	public void EnableBackpack()
	{
		SetBackpack(useBackpack: true);
	}

	public void DisableBackpack()
	{
		SetBackpack(useBackpack: false);
	}

	private void SetBackpack(bool useBackpack)
	{
		IsBackpackEnabled = useBackpack;
		BackpackChanged?.Invoke(this, EventArgs.Empty);
	}
}
