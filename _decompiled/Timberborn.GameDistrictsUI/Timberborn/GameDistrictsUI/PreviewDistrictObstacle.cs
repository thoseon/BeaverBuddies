using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.GameDistricts;

namespace Timberborn.GameDistrictsUI;

internal class PreviewDistrictObstacle : BaseComponent, IAwakableComponent, IPreviewServiceMember
{
	private DistrictObstacle _districtObstacle;

	private bool _isAdded;

	public void Awake()
	{
		_districtObstacle = GetComponent<DistrictObstacle>();
	}

	public void AddToPreviewService()
	{
		if (!_isAdded)
		{
			_districtObstacle.AddToPreviewDistricts();
			_isAdded = true;
		}
	}

	public void RemoveFromPreviewService()
	{
		if (_isAdded)
		{
			_districtObstacle.RemoveFromPreviewDistricts();
			_isAdded = false;
		}
	}
}
