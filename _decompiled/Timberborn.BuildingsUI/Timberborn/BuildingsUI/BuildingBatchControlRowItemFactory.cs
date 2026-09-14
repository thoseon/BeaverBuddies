using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.Buildings;
using Timberborn.BuildingsNavigation;
using Timberborn.ConstructionSites;
using Timberborn.ConstructionSitesUI;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.BuildingsUI;

public class BuildingBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly DistanceToColorConverter _distanceToColorConverter;

	private readonly ILoc _loc;

	public BuildingBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, EntitySelectionService entitySelectionService, DistanceToColorConverter distanceToColorConverter, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_entitySelectionService = entitySelectionService;
		_distanceToColorConverter = distanceToColorConverter;
		_loc = loc;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		LabeledEntity component = entity.GetComponent<LabeledEntity>();
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/BatchControl/BuildingBatchControlRowItem");
		visualElement.Q<Button>("Select").RegisterCallback<ClickEvent>(delegate
		{
			_entitySelectionService.SelectAndFocusOn(entity);
		});
		Image image = visualElement.Q<Image>("Image");
		image.sprite = component.Image;
		_tooltipRegistrar.Register(image, component.DisplayName);
		DistrictBuildingDistance districtBuildingDistance = entity.GetComponent<DistrictBuildingDistance>();
		Label label = visualElement.Q<Label>("DistanceText");
		_tooltipRegistrar.RegisterUpdatable(label, () => districtBuildingDistance.DescribeDistance());
		PausableBuilding component2 = entity.GetComponent<PausableBuilding>();
		VisualElement visualElement2 = visualElement.Q<VisualElement>("PausableWrapper");
		Toggle pausableToggle = visualElement.Q<Toggle>("PausableToggle");
		InitializePausableBuilding(component2, visualElement2, pausableToggle);
		ConstructionSite component3 = entity.GetComponent<ConstructionSite>();
		ConstructionSiteDescriber constructionSiteDescriber = ((BaseComponent)(object)component3).GetComponent<ConstructionSiteDescriber>();
		VisualElement visualElement3 = visualElement.Q<VisualElement>("ConstructionWrapper");
		Label constructionProgressLabel = visualElement.Q<Label>("ProgressText");
		VisualElement constructionProgressBar = visualElement.Q<VisualElement>("Progress");
		_tooltipRegistrar.RegisterUpdatable(visualElement3, () => constructionSiteDescriber.GetProgressInfoFull());
		return new BuildingBatchControlRowItem(visualElement, _loc, _distanceToColorConverter, districtBuildingDistance, label, visualElement2, pausableToggle, component2, component3, visualElement3, constructionProgressLabel, constructionProgressBar);
	}

	private static void InitializePausableBuilding(PausableBuilding pausableBuilding, VisualElement toggleWrapper, Toggle pausableToggle)
	{
		bool visible = (bool)pausableBuilding && pausableBuilding.IsPausable();
		toggleWrapper.ToggleDisplayStyle(visible);
		if ((bool)pausableBuilding)
		{
			pausableToggle.SetValueWithoutNotify(!pausableBuilding.Paused);
			pausableToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
			{
				ToggleActivationState(evt.newValue, pausableBuilding);
			});
		}
	}

	private static void ToggleActivationState(bool resume, PausableBuilding pausableBuilding)
	{
		if (resume)
		{
			pausableBuilding.Resume();
		}
		else
		{
			pausableBuilding.Pause();
		}
	}
}
