using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Navigation;

internal class DistrictUpdater
{
	private readonly DistrictMap _districtMap;

	private readonly InstantDistrictMap _instantDistrictMap;

	private readonly PreviewDistrictMap _previewDistrictMap;

	private readonly DistrictObstacleService _districtObstacleService;

	private readonly InstantDistrictObstacleService _instantDistrictObstacleService;

	private readonly PreviewDistrictObstacleService _previewDistrictObstacleService;

	private readonly Queue<DistrictChange> _enqueuedRegularChanges = new Queue<DistrictChange>();

	private readonly Queue<DistrictChange> _enqueuedInstantChanges = new Queue<DistrictChange>();

	public DistrictUpdater(DistrictMap districtMap, InstantDistrictMap instantDistrictMap, PreviewDistrictMap previewDistrictMap, DistrictObstacleService districtObstacleService, InstantDistrictObstacleService instantDistrictObstacleService, PreviewDistrictObstacleService previewDistrictObstacleService)
	{
		_districtMap = districtMap;
		_instantDistrictMap = instantDistrictMap;
		_previewDistrictMap = previewDistrictMap;
		_districtObstacleService = districtObstacleService;
		_instantDistrictObstacleService = instantDistrictObstacleService;
		_previewDistrictObstacleService = previewDistrictObstacleService;
	}

	public void EnqueueChange(DistrictChange districtChange)
	{
		_enqueuedInstantChanges.Enqueue(districtChange);
		_enqueuedRegularChanges.Enqueue(districtChange);
	}

	public void ApplyPreviewChange(DistrictChange districtChange)
	{
		districtChange.ApplyChange(_previewDistrictMap, _previewDistrictObstacleService);
	}

	public void ProcessRegularChanges(NavMeshUpdate.Builder navMeshUpdateBuilder)
	{
		if (!_enqueuedRegularChanges.IsEmpty())
		{
			while (!_enqueuedRegularChanges.IsEmpty())
			{
				_enqueuedRegularChanges.Dequeue().ApplyChange(_districtMap, _districtObstacleService, navMeshUpdateBuilder);
			}
		}
	}

	public void ProcessInstantChanges(NavMeshUpdate.Builder navMeshUpdateBuilder)
	{
		if (!_enqueuedInstantChanges.IsEmpty())
		{
			while (!_enqueuedInstantChanges.IsEmpty())
			{
				DistrictChange districtChange = _enqueuedInstantChanges.Dequeue();
				districtChange.ApplyChange(_instantDistrictMap, _instantDistrictObstacleService, navMeshUpdateBuilder);
				ApplyPreviewChange(districtChange);
			}
		}
	}
}
