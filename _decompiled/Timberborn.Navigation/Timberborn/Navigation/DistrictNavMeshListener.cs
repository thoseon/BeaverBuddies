namespace Timberborn.Navigation;

internal class DistrictNavMeshListener : IPrioritizedSingletonNavMeshListener, IPrioritizedSingletonPreviewNavMeshListener, IPrioritizedSingletonInstantNavMeshListener
{
	private readonly DistrictMap _districtMap;

	private readonly PreviewDistrictMap _previewDistrictMap;

	private readonly InstantDistrictMap _instantDistrictMap;

	public DistrictNavMeshListener(DistrictMap districtMap, PreviewDistrictMap previewDistrictMap, InstantDistrictMap instantDistrictMap)
	{
		_districtMap = districtMap;
		_previewDistrictMap = previewDistrictMap;
		_instantDistrictMap = instantDistrictMap;
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_districtMap.OnNavMeshUpdated(navMeshUpdate);
	}

	public void OnPreviewNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_previewDistrictMap.OnNavMeshUpdated(navMeshUpdate);
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_instantDistrictMap.OnNavMeshUpdated(navMeshUpdate);
	}
}
