using Timberborn.BaseComponentSystem;
using Timberborn.BuildingsNavigation;
using Timberborn.EntityNaming;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.GameDistrictsUI;

internal class DistrictBuildingEntityBadge : BaseComponent, IAwakableComponent, IEntityBadge
{
	private readonly EntitySelectionService _entitySelectionService;

	private readonly DistanceToColorConverter _distanceToColorConverter;

	private LabeledEntity _labeledEntity;

	private NamedEntity _namedEntity;

	private DistrictBuilding _districtBuilding;

	private DistrictBuildingDistance _districtBuildingDistance;

	public int EntityBadgePriority => 110;

	public DistrictBuildingEntityBadge(EntitySelectionService entitySelectionService, DistanceToColorConverter distanceToColorConverter)
	{
		_entitySelectionService = entitySelectionService;
		_distanceToColorConverter = distanceToColorConverter;
	}

	public void Awake()
	{
		_labeledEntity = GetComponent<LabeledEntity>();
		_namedEntity = GetComponent<NamedEntity>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		_districtBuildingDistance = GetComponent<DistrictBuildingDistance>();
	}

	public string GetEntitySubtitle()
	{
		NamedEntity namedEntity = _namedEntity;
		if (namedEntity == null || !namedEntity.IsEditable)
		{
			return "";
		}
		return _labeledEntity.DisplayName;
	}

	public ClickableSubtitle GetEntityClickableSubtitle()
	{
		DistrictCenter district = _districtBuilding.GetInstantOrConstructionDistrict();
		if (district != null)
		{
			return ClickableSubtitle.Create(delegate
			{
				_entitySelectionService.SelectAndFocusOn(district);
			}, GetSubtitleText(district), _districtBuildingDistance.DescribeDistance(), _districtBuildingDistance.IsAboveThreshold());
		}
		return ClickableSubtitle.CreateEmpty();
	}

	public Sprite GetEntityAvatar()
	{
		return _labeledEntity.Image;
	}

	private string GetSubtitleText(DistrictCenter district)
	{
		if (_districtBuildingDistance.TryGetDistanceToDistrict(out var distance))
		{
			string arg = ColorUtility.ToHtmlStringRGB(_distanceToColorConverter.DistanceToColor(distance));
			return $"{district.DistrictName} <color=#{arg}>({distance})</color>";
		}
		return district.DistrictName;
	}
}
