using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.RecoveredGoodSystem;

internal class NoStorageStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string NoStorageLocKey = "Status.RecoveredGoodStack.NoStorage";

	private readonly ILoc _loc;

	private StatusToggle _noStorageStatusToggle;

	public NoStorageStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_noStorageStatusToggle = StatusToggle.CreateNormalStatus("NoStorage", _loc.T(NoStorageLocKey));
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_noStorageStatusToggle);
	}

	public void ActivateNoStorageStatus()
	{
		_noStorageStatusToggle.Activate();
	}

	public void DeactivateNoStorageStatus()
	{
		_noStorageStatusToggle.Deactivate();
	}
}
