using Timberborn.BlockObjectTools;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.ToolSystemUI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.GameStartup;

internal class StartingBuildingToolDescriber : IBlockObjectToolDescriber, ILoadableSingleton
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly StartingBuildingSpawner _startingBuildingSpawner;

	private readonly ILoc _loc;

	private Sprite _iconSprite;

	public StartingBuildingToolDescriber(VisualElementLoader visualElementLoader, StartingBuildingSpawner startingBuildingSpawner, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_startingBuildingSpawner = startingBuildingSpawner;
		_loc = loc;
	}

	public ToolDescription Describe(BlockObjectTool blockObjectTool, IBlockObjectPlacer blockObjectPlacer)
	{
		string elementName = "Game/EntityDescription/DescriptionEmptySection";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		AddHeaderSection(visualElement);
		ToolDescription.Builder builder = new ToolDescription.Builder();
		if (visualElement.childCount > 0)
		{
			builder.AddSection(visualElement);
		}
		return builder.Build();
	}

	public void Load()
	{
		TemplateSpec startingBuildingTemplateSpec = _startingBuildingSpawner.StartingBuildingTemplateSpec;
		_iconSprite = startingBuildingTemplateSpec.GetSpec<LabeledEntitySpec>().Icon.Asset;
	}

	private void AddHeaderSection(VisualElement root)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityDescription/DescriptionHeader");
		visualElement.Q<Label>("Title").text = _loc.T("FlexibleStart.ChooseStartLocation");
		visualElement.Q<Image>("Icon").sprite = _iconSprite;
		root.Add(visualElement);
	}
}
