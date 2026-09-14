using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.GameDistricts;

public class PreviewDistrictAdder : BaseComponent, IAwakableComponent, IPreviewServiceMember, IPreviewSelectionListener
{
	private readonly IDistrictService _districtService;

	private DistrictCenter _districtCenter;

	private Preview _preview;

	private District _district;

	public PreviewDistrictAdder(IDistrictService districtService)
	{
		_districtService = districtService;
	}

	public void Awake()
	{
		_districtCenter = GetComponent<DistrictCenter>();
		_preview = GetComponent<Preview>();
	}

	public void OnPreviewSelect()
	{
		if (_preview.PreviewState.IsBuildable)
		{
			AddToPreviewDistrict();
		}
	}

	public void OnPreviewUnselect()
	{
	}

	public void AddToPreviewService()
	{
	}

	public void RemoveFromPreviewService()
	{
		RemoveFromDistrict();
	}

	private void AddToPreviewDistrict()
	{
		Vector3Int centerCoordinates = _districtCenter.CenterCoordinates;
		_district = _districtService.AddPreviewDistrict(centerCoordinates);
	}

	private void RemoveFromDistrict()
	{
		if (_district != null)
		{
			_districtService.RemovePreviewDistrict(_district);
			_district = null;
		}
	}
}
