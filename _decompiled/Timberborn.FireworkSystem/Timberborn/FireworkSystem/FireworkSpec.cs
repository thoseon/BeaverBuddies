using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;
using Timberborn.TemplateSystem;

namespace Timberborn.FireworkSystem;

public record FireworkSpec : ComponentSpec
{
	[Serialize("DisplayNameLocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize]
	public bool HasBurst { get; init; }

	[Serialize]
	public string TrailSound { get; init; }

	[Serialize]
	public string BurstSound { get; init; }

	[Serialize]
	private string DisplayNameLocKey { get; init; }

	public string FireworkId => base.Blueprint.GetSpec<TemplateSpec>().TemplateName;
}
