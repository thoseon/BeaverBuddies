using UnityEngine;

namespace Timberborn.Navigation;

public interface IDistrictService
{
	District AddDistrict(Vector3Int centerCoordinates);

	void RemoveDistrict(District district);

	District AddPreviewDistrict(Vector3Int centerCoordinates);

	void RemovePreviewDistrict(District district);

	void SetObstacle(Vector3Int coordinates);

	void UnsetObstacle(Vector3Int coordinates);

	void SetPreviewObstacle(Vector3Int coordinates);

	void UnsetPreviewObstacle(Vector3Int coordinates);

	bool IsPreviewDistrictInConflict(Vector3Int? previewDistrictCenter);

	bool DistrictIsGloballyReachable(District district, Vector3 start);

	bool IsOnDistrictRoad(District district, Vector3 road);

	bool IsOnInstantDistrictRoad(District district, Vector3 road);

	bool IsOnPreviewDistrictRoad(District district, Vector3 road);

	bool IsOnInstantDistrictRoadSpill(Accessible accessible);

	bool IsOnInstantDistrictRoadSpill(Vector3 position);

	Vector3 GetRandomDestinationInDistrict(District district, Vector3 coordinates);
}
