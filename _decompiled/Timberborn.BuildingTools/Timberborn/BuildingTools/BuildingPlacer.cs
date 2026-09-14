using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlockObjectTools;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.ConstructionSites;
using Timberborn.Coordinates;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.ScienceSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.BuildingTools;

public class BuildingPlacer : IBlockObjectPlacer
{
	private static readonly string PlaceFinishedKey = "PlaceFinished";

	private static readonly string UnlockableClass = "unlockable";

	private readonly BuildingCostSectionProvider _buildingCostSectionProvider;

	private readonly ConstructionFactory _constructionFactory;

	private readonly InputService _inputService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ScienceService _scienceService;

	private readonly ToolUnlockingService _toolUnlockingService;

	private readonly UnlockSectionController _unlockSectionController;

	private readonly ILoc _loc;

	private readonly ImmutableArray<ISectionProvider> _sectionProviders;

	private readonly Phrase _scienceCostPhrase = Phrase.New().FormatCompact();

	public BuildingPlacer(BuildingCostSectionProvider buildingCostSectionProvider, ConstructionFactory constructionFactory, InputService inputService, VisualElementLoader visualElementLoader, ScienceService scienceService, ToolUnlockingService toolUnlockingService, UnlockSectionController unlockSectionController, ILoc loc, IEnumerable<ISectionProvider> sectionProviders)
	{
		_buildingCostSectionProvider = buildingCostSectionProvider;
		_constructionFactory = constructionFactory;
		_inputService = inputService;
		_visualElementLoader = visualElementLoader;
		_scienceService = scienceService;
		_toolUnlockingService = toolUnlockingService;
		_unlockSectionController = unlockSectionController;
		_loc = loc;
		_sectionProviders = sectionProviders.ToImmutableArray();
	}

	public bool CanHandle(BlockObjectSpec template)
	{
		return template.HasSpec<BuildingSpec>();
	}

	public void Place(EntitySetup.Builder entitySetupBuilder, Placement placement)
	{
		BuildingSpec spec = entitySetupBuilder.Template.GetSpec<BuildingSpec>();
		if (ShouldBePlacedFinished(spec))
		{
			_constructionFactory.CreateAsFinished(entitySetupBuilder, placement);
		}
		else
		{
			_constructionFactory.CreateAsUnfinished(entitySetupBuilder, placement);
		}
	}

	public void Describe(BlockObjectTool tool, ToolDescription.Builder builder, Preview preview)
	{
		AddBuildingCostSection(builder, preview);
		AddSections(builder, preview);
		AddUnlockSection(tool, builder);
	}

	private bool ShouldBePlacedFinished(BuildingSpec buildingSpec)
	{
		if (!_inputService.IsKeyHeld(PlaceFinishedKey))
		{
			return buildingSpec.PlaceFinished;
		}
		return true;
	}

	private void AddBuildingCostSection(ToolDescription.Builder builder, Preview preview)
	{
		if (_buildingCostSectionProvider.TryGetSection(preview, out var section))
		{
			builder.AddExternalSection(section);
		}
	}

	private void AddSections(ToolDescription.Builder builder, Preview preview)
	{
		foreach (ISectionProvider sectionProvider in _sectionProviders)
		{
			if (sectionProvider.TryGetSection(preview, out var section))
			{
				builder.AddSection(section);
			}
		}
	}

	private void AddUnlockSection(BlockObjectTool tool, ToolDescription.Builder builder)
	{
		if (_toolUnlockingService.IsLocked(tool))
		{
			VisualElement root = _visualElementLoader.LoadVisualElement("Game/ToolPanel/UnlockSection");
			BuildingSpec spec = tool.Template.GetSpec<BuildingSpec>();
			InitializeUnlockButton(tool, spec, root.Q<VisualElement>("UnlockButton"));
			builder.AddUpdatableSection(root, delegate
			{
				_unlockSectionController.UpdateSection(root, tool);
			});
		}
	}

	private void InitializeUnlockButton(BlockObjectTool tool, BuildingSpec buildingSpec, VisualElement unlockButton)
	{
		int buildingScienceCost = buildingSpec.ScienceCost;
		VisualElement visualElement = unlockButton.Q<VisualElement>("ScienceCostSection");
		visualElement.Q<Label>("ScienceCost").text = _loc.T(_scienceCostPhrase, buildingScienceCost);
		visualElement.RegisterCallback<MouseEnterEvent>(delegate
		{
			unlockButton.EnableInClassList(UnlockableClass, _scienceService.SciencePoints >= buildingScienceCost);
		});
		visualElement.RegisterCallback<ClickEvent>(delegate
		{
			_toolUnlockingService.TryToUnlock(tool);
		});
	}
}
