using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Navigation;

namespace Timberborn.GoodStackSystem;

internal class GoodStackAccessible : BaseComponent, IAwakableComponent, IAccessibleNeeder
{
	private Accessible _accessible;

	private BlockObjectCenter _blockObjectCenter;

	public string AccessibleComponentName => "GoodStack";

	public void Awake()
	{
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
	}

	public void SetAccessible(Accessible accessible)
	{
		_accessible = accessible;
	}

	public void Enable()
	{
		_accessible.SetAccesses(Enumerables.One(_blockObjectCenter.WorldCenterGrounded));
	}

	public void Disable()
	{
		_accessible.ClearAccesses();
	}
}
