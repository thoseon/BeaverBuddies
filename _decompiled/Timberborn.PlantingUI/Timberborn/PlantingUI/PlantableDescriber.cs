using System.Collections.Generic;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Gathering;
using Timberborn.GatheringUI;
using Timberborn.Goods;
using Timberborn.Growing;
using Timberborn.GrowingUI;
using Timberborn.Localization;
using Timberborn.Planting;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.ToolSystemUI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.PlantingUI;

public class PlantableDescriber : ILoadableSingleton
{
	private static readonly string TwoItemsClass = "two-items";

	private static readonly string DescriptionLocKey = "PlantingTool.Description";

	private static readonly string RequiredBuildingLocKey = "PlantingTool.RequiredBuilding";

	private readonly EntityDescriptionService _entityDescriptionService;

	private readonly TemplateInstantiator _templateInstantiator;

	private readonly ILoc _loc;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly GrowableToolPanelItemFactory _growableToolPanelItemFactory;

	private readonly GatherableToolPanelItemFactory _gatherableToolPanelItemFactory;

	private readonly IGoodService _goodService;

	private readonly RootObjectProvider _rootObjectProvider;

	private Transform _parent;

	private readonly Dictionary<PlantableSpec, Plantable> _previewCache = new Dictionary<PlantableSpec, Plantable>();

	public PlantableDescriber(EntityDescriptionService entityDescriptionService, TemplateInstantiator templateInstantiator, ILoc loc, VisualElementLoader visualElementLoader, GrowableToolPanelItemFactory growableToolPanelItemFactory, GatherableToolPanelItemFactory gatherableToolPanelItemFactory, IGoodService goodService, RootObjectProvider rootObjectProvider)
	{
		_entityDescriptionService = entityDescriptionService;
		_templateInstantiator = templateInstantiator;
		_loc = loc;
		_visualElementLoader = visualElementLoader;
		_growableToolPanelItemFactory = growableToolPanelItemFactory;
		_gatherableToolPanelItemFactory = gatherableToolPanelItemFactory;
		_goodService = goodService;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("PlantableDescriber").transform;
	}

	public ToolDescription Describe(PlantableSpec plantableSpec, string buildingName)
	{
		Plantable previewFromTemplate = GetPreviewFromTemplate(plantableSpec);
		string elementName = "Game/EntityDescription/DescriptionEmptySection";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		_entityDescriptionService.DescribeAsSeparateSections(previewFromTemplate, visualElement, GetDescription(buildingName));
		return new ToolDescription.Builder().AddSection(visualElement).AddSection(GetYieldSection(plantableSpec)).Build();
	}

	private string GetDescription(string buildingName)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(_loc.T(DescriptionLocKey));
		stringBuilder.Append(SpecialStrings.RowStarter + _loc.T(RequiredBuildingLocKey, buildingName));
		return stringBuilder.ToString();
	}

	private Plantable GetPreviewFromTemplate(PlantableSpec plantableSpec)
	{
		return _previewCache.GetOrAdd(plantableSpec, () => Create(plantableSpec));
	}

	private Plantable Create(PlantableSpec plantableSpec)
	{
		GameObject gameObject = _templateInstantiator.Instantiate(plantableSpec.Blueprint, _parent);
		gameObject.SetActive(value: false);
		return gameObject.GetComponentSlow<Plantable>();
	}

	private VisualElement GetYieldSection(PlantableSpec plantableSpec)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/ToolPanel/ResourceYieldPanel");
		VisualElement child = _growableToolPanelItemFactory.Create(plantableSpec.GetSpec<GrowableSpec>());
		visualElement.Add(child);
		GatherableSpec spec = plantableSpec.GetSpec<GatherableSpec>();
		if ((object)spec != null && _goodService.HasGood(spec.Yielder.Yield.Id))
		{
			VisualElement child2 = _gatherableToolPanelItemFactory.Create(spec);
			visualElement.Add(child2);
			visualElement.AddToClassList(TwoItemsClass);
		}
		return visualElement;
	}
}
