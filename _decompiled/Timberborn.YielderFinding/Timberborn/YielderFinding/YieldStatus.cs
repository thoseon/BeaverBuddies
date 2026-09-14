using Timberborn.BaseComponentSystem;
using Timberborn.WorkSystem;

namespace Timberborn.YielderFinding;

public class YieldStatus : BaseComponent, IAwakableComponent
{
	private NothingToDoInRangeStatus _nothingToDoInRangeStatus;

	public void Awake()
	{
		_nothingToDoInRangeStatus = GetComponent<NothingToDoInRangeStatus>();
	}

	public void UpdateStatus(YielderSearchResult yielderSearchResult)
	{
		if (yielderSearchResult.NoYielderInRange)
		{
			_nothingToDoInRangeStatus.ActivateStatus();
		}
		else
		{
			_nothingToDoInRangeStatus.DeactivateStatus();
		}
	}
}
