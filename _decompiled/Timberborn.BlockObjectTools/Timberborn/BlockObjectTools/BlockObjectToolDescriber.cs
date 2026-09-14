using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.MapStateSystem;
using Timberborn.ToolSystemUI;
using UnityEngine.UIElements;

namespace Timberborn.BlockObjectTools;

public class BlockObjectToolDescriber : IBlockObjectToolDescriber
{
	private readonly EntityDescriptionService _entityDescriptionService;

	private readonly PreviewFactory _previewFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly MapEditorMode _mapEditorMode;

	private readonly Dictionary<PlaceableBlockObjectSpec, Preview> _previewCache = new Dictionary<PlaceableBlockObjectSpec, Preview>();

	public BlockObjectToolDescriber(EntityDescriptionService entityDescriptionService, PreviewFactory previewFactory, VisualElementLoader visualElementLoader, MapEditorMode mapEditorMode)
	{
		_entityDescriptionService = entityDescriptionService;
		_previewFactory = previewFactory;
		_visualElementLoader = visualElementLoader;
		_mapEditorMode = mapEditorMode;
	}

	public ToolDescription Describe(BlockObjectTool blockObjectTool, IBlockObjectPlacer blockObjectPlacer)
	{
		PlaceableBlockObjectSpec template = blockObjectTool.Template;
		Preview previewFromTemplate = GetPreviewFromTemplate(template);
		string elementName = "Game/EntityDescription/DescriptionEmptySection";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		_entityDescriptionService.DescribeAsSeparateSections(previewFromTemplate, visualElement);
		ToolDescription.Builder builder = new ToolDescription.Builder();
		if (visualElement.childCount > 0)
		{
			builder.AddSection(visualElement);
		}
		blockObjectPlacer.Describe(blockObjectTool, builder, previewFromTemplate);
		if (template.DevModeTool && !_mapEditorMode.IsMapEditor)
		{
			string text = "<color=#ff0000><b>This is a DevModeTool</b></color>";
			builder.AddPrioritizedSection(text.ToUpper());
		}
		return builder.Build();
	}

	private Preview GetPreviewFromTemplate(PlaceableBlockObjectSpec template)
	{
		return _previewCache.GetOrAdd(template, () => _previewFactory.Create(template));
	}
}
