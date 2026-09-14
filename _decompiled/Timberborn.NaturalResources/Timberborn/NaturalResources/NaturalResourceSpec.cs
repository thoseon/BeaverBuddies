using Timberborn.BlueprintSystem;
using Timberborn.TemplateSystem;
using Timberborn.Yielding;

namespace Timberborn.NaturalResources;

public record NaturalResourceSpec : ComponentSpec, IOrderableYielder
{
	[Serialize]
	public int Order { get; init; }

	public bool UsableWithCurrentFeatureToggles => GetSpec<TemplateSpec>().UsableWithCurrentFeatureToggles;
}
