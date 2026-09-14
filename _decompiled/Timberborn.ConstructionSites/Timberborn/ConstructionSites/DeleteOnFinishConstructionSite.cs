using System;
using Timberborn.BaseComponentSystem;

namespace Timberborn.ConstructionSites;

public class DeleteOnFinishConstructionSite : BaseComponent
{
	public event EventHandler Deleted;

	public void NotifyDeleted()
	{
		Deleted?.Invoke(this, EventArgs.Empty);
	}
}
