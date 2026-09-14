using Timberborn.BlueprintSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.BlockSystem;

public record PlaceableBlockObjectSpec : ComponentSpec
{
	[Serialize]
	public string ToolGroupId { get; init; }

	[Serialize]
	public int ToolOrder { get; init; }

	[Serialize]
	public ToolShapes ToolShape { get; init; }

	[Serialize]
	public BlockObjectLayout Layout { get; init; }

	[Serialize]
	public CustomPivotSpec CustomPivot { get; init; }

	[Serialize]
	public bool CanBeAttachedToTerrainSide { get; init; }

	[Serialize]
	public bool DevModeTool { get; init; }

	public bool UsableWithCurrentFeatureToggles => GetSpec<TemplateSpec>().UsableWithCurrentFeatureToggles;
}
