using Timberborn.BlueprintSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.Planting;

public record PlantableSpec : ComponentSpec
{
	[Serialize]
	public string ResourceGroup { get; init; }

	[Serialize]
	public float PlantTimeInHours { get; init; }

	public string TemplateName => GetSpec<TemplateSpec>().TemplateName;
}
