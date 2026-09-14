using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.BlockSystemNavigation;

public class BlockObjectNavMeshGroupInitializer : ILoadableSingleton
{
	private readonly TemplateService _templateService;

	private readonly NavMeshGroupService _navMeshGroupService;

	public BlockObjectNavMeshGroupInitializer(TemplateService templateService, NavMeshGroupService navMeshGroupService)
	{
		_templateService = templateService;
		_navMeshGroupService = navMeshGroupService;
	}

	public void Load()
	{
		foreach (BlockObjectNavMeshSettingsSpec item in _templateService.GetAll<BlockObjectNavMeshSettingsSpec>())
		{
			foreach (BlockObjectNavMeshEdgeGroupSpec edgeGroup in item.EdgeGroups)
			{
				if (edgeGroup.UseGroup)
				{
					_navMeshGroupService.GetOrAddGroupId(edgeGroup.GroupName);
				}
			}
		}
	}
}
