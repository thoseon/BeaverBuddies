using Timberborn.BlockSystem;
using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.GameDistrictsUI;

internal class DistrictPreviewsValidator : IBlockObjectValidator
{
	private static readonly string ErrorMessageLocKey = "BuildingTools.DistrictsInConflict";

	private readonly IDistrictService _districtService;

	private readonly ILoc _loc;

	public DistrictPreviewsValidator(IDistrictService districtService, ILoc loc)
	{
		_districtService = districtService;
		_loc = loc;
	}

	public bool IsValid(BlockObject blockObject, out string errorMessage)
	{
		if (IsPreviewDistrictInConflict(blockObject))
		{
			errorMessage = _loc.T(ErrorMessageLocKey);
			return false;
		}
		errorMessage = null;
		return true;
	}

	private bool IsPreviewDistrictInConflict(BlockObject blockObject)
	{
		if (blockObject.IsPreview)
		{
			Vector3Int? previewDistrictCenter = blockObject.GetComponent<DistrictCenter>()?.CenterCoordinates;
			return _districtService.IsPreviewDistrictInConflict(previewDistrictCenter);
		}
		return false;
	}
}
