using System.Collections.Generic;
using Timberborn.BlockObjectTools;
using Timberborn.BottomBarSystem;
using Timberborn.MapEditorBrushesUI;
using Timberborn.MapEditorNaturalResourcesUI;
using Timberborn.MapMetadataSystemUI;
using Timberborn.MapThumbnailCapturingUI;
using Timberborn.ToolButtonSystem;

namespace Timberborn.MapEditorUI;

internal class MapEditorToolButtons : IBottomBarElementsProvider
{
	private static readonly string AbsoluteTerrainHeightBrushImageKey = "AbsoluteTerrainHeightBrush";

	private static readonly string RelativeTerrainHeightBrushImageKey = "RelativeTerrainHeightBrushIcon";

	private static readonly string SculptingTerrainBrushImageKey = "SculptingTerrainBrushIcon";

	private static readonly string NaturalResourcesSpawningImageKey = "NaturalResourcesIcon";

	private static readonly string NaturalResourcesRemovalImageKey = "RemoveNaturalResourcesIcon";

	private static readonly string DeleteBuildingImageKey = "DeleteObjectIcon";

	private static readonly string ThumbnailCapturingImageKey = "ThumbnailCapturingIcon";

	private static readonly string MapMetadataImageKey = "MapMetadataIcon";

	private readonly ToolButtonFactory _toolButtonFactory;

	private readonly AbsoluteTerrainHeightBrushTool _absoluteTerrainHeightBrushTool;

	private readonly RelativeTerrainHeightBrushTool _relativeTerrainHeightBrushTool;

	private readonly SculptingTerrainBrushTool _sculptingTerrainBrushTool;

	private readonly NaturalResourceSpawningBrushTool _naturalResourceSpawningBrushTool;

	private readonly NaturalResourceRemovalBrushTool _naturalResourceRemovalBrushTool;

	private readonly EntityBlockObjectDeletionTool _entityBlockObjectDeletionTool;

	private readonly ThumbnailCapturingTool _thumbnailCapturingTool;

	private readonly MapMetadataTool _mapMetadataTool;

	public MapEditorToolButtons(ToolButtonFactory toolButtonFactory, AbsoluteTerrainHeightBrushTool absoluteTerrainHeightBrushTool, RelativeTerrainHeightBrushTool relativeTerrainHeightBrushTool, SculptingTerrainBrushTool sculptingTerrainBrushTool, NaturalResourceSpawningBrushTool naturalResourceSpawningBrushTool, NaturalResourceRemovalBrushTool naturalResourceRemovalBrushTool, EntityBlockObjectDeletionTool entityBlockObjectDeletionTool, ThumbnailCapturingTool thumbnailCapturingTool, MapMetadataTool mapMetadataTool)
	{
		_toolButtonFactory = toolButtonFactory;
		_absoluteTerrainHeightBrushTool = absoluteTerrainHeightBrushTool;
		_relativeTerrainHeightBrushTool = relativeTerrainHeightBrushTool;
		_sculptingTerrainBrushTool = sculptingTerrainBrushTool;
		_naturalResourceSpawningBrushTool = naturalResourceSpawningBrushTool;
		_naturalResourceRemovalBrushTool = naturalResourceRemovalBrushTool;
		_entityBlockObjectDeletionTool = entityBlockObjectDeletionTool;
		_thumbnailCapturingTool = thumbnailCapturingTool;
		_mapMetadataTool = mapMetadataTool;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		ToolButton toolButton = _toolButtonFactory.CreateGrouplessBlue(_absoluteTerrainHeightBrushTool, AbsoluteTerrainHeightBrushImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton.Root);
		ToolButton toolButton2 = _toolButtonFactory.CreateGrouplessBlue(_relativeTerrainHeightBrushTool, RelativeTerrainHeightBrushImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton2.Root);
		ToolButton toolButton3 = _toolButtonFactory.CreateGrouplessBlue(_sculptingTerrainBrushTool, SculptingTerrainBrushImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton3.Root);
		ToolButton toolButton4 = _toolButtonFactory.CreateGrouplessBlue(_naturalResourceSpawningBrushTool, NaturalResourcesSpawningImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton4.Root);
		ToolButton toolButton5 = _toolButtonFactory.CreateGrouplessBlue(_naturalResourceRemovalBrushTool, NaturalResourcesRemovalImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton5.Root);
		ToolButton toolButton6 = _toolButtonFactory.CreateGrouplessBlue(_entityBlockObjectDeletionTool, DeleteBuildingImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton6.Root);
		ToolButton toolButton7 = _toolButtonFactory.CreateGrouplessBlue(_thumbnailCapturingTool, ThumbnailCapturingImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton7.Root);
		ToolButton toolButton8 = _toolButtonFactory.CreateGrouplessBlue(_mapMetadataTool, MapMetadataImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton8.Root);
	}
}
