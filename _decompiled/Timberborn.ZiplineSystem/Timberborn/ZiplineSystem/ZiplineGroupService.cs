using Timberborn.Navigation;
using Timberborn.SingletonSystem;

namespace Timberborn.ZiplineSystem;

public class ZiplineGroupService : ILoadableSingleton
{
	private static readonly string RegularGroupName = "Zipline";

	private static readonly string PathStartGroupName = "ZiplinePathStart";

	private static readonly string TurnGroupName = "ZiplineTurn";

	private readonly NavMeshGroupService _navMeshGroupService;

	public int RegularGroupId { get; private set; }

	public int PathStartGroupId { get; private set; }

	public int TurnGroupId { get; private set; }

	public ZiplineGroupService(NavMeshGroupService navMeshGroupService)
	{
		_navMeshGroupService = navMeshGroupService;
	}

	public void Load()
	{
		RegularGroupId = _navMeshGroupService.GetOrAddGroupId(RegularGroupName);
		PathStartGroupId = _navMeshGroupService.GetOrAddGroupId(PathStartGroupName);
		TurnGroupId = _navMeshGroupService.GetOrAddGroupId(TurnGroupName);
	}

	public bool IsRegularEdge(int fromGroupId, int toGroupId)
	{
		if (fromGroupId == RegularGroupId)
		{
			return toGroupId == RegularGroupId;
		}
		return false;
	}

	public bool IsTurnEdge(int fromGroupId, int toGroupId)
	{
		if (fromGroupId == RegularGroupId)
		{
			return toGroupId == TurnGroupId;
		}
		return false;
	}

	public bool IsAnyZiplineGroup(int groupId)
	{
		if (groupId != RegularGroupId && groupId != PathStartGroupId)
		{
			return groupId == TurnGroupId;
		}
		return true;
	}
}
