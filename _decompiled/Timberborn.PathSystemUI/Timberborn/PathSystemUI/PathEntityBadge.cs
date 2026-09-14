using Timberborn.BaseComponentSystem;
using Timberborn.BuildingsNavigation;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.PathSystemUI;

internal class PathEntityBadge : BaseComponent, IAwakableComponent, IEntityBadge
{
	private readonly EntitySelectionService _entitySelectionService;

	private readonly DistanceToColorConverter _distanceToColorConverter;

	private readonly DistanceToDistrictDescriber _distanceToDistrictDescriber;

	private LabeledEntity _labeledEntity;

	private PathDistrictRetriever _pathDistrictRetriever;

	public int EntityBadgePriority => 100;

	public PathEntityBadge(EntitySelectionService entitySelectionService, DistanceToColorConverter distanceToColorConverter, DistanceToDistrictDescriber distanceToDistrictDescriber)
	{
		_entitySelectionService = entitySelectionService;
		_distanceToColorConverter = distanceToColorConverter;
		_distanceToDistrictDescriber = distanceToDistrictDescriber;
	}

	public void Awake()
	{
		_labeledEntity = GetComponent<LabeledEntity>();
		_pathDistrictRetriever = GetComponent<PathDistrictRetriever>();
	}

	public string GetEntitySubtitle()
	{
		return "";
	}

	public ClickableSubtitle GetEntityClickableSubtitle()
	{
		if (_pathDistrictRetriever.TryGetDistanceToDistrictCenter(out var district, out var distance))
		{
			return ClickableSubtitle.Create(delegate
			{
				_entitySelectionService.SelectAndFocusOn(district);
			}, GetSubtitleText(district, distance), _distanceToDistrictDescriber.DescribeDistance(distance), isWarning: false);
		}
		return ClickableSubtitle.CreateEmpty();
	}

	public Sprite GetEntityAvatar()
	{
		return _labeledEntity.Image;
	}

	private string GetSubtitleText(DistrictCenter district, float distance)
	{
		string arg = ColorUtility.ToHtmlStringRGB(_distanceToColorConverter.DistanceToColor(distance));
		int num = Mathf.RoundToInt(distance);
		return $"{district.DistrictName} <color=#{arg}>({num})</color>";
	}
}
