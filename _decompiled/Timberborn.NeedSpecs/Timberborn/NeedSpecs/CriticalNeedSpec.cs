using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;

namespace Timberborn.NeedSpecs;

public record CriticalNeedSpec : ComponentSpec
{
	[Serialize]
	public CriticalNeedType CriticalNeedType { get; init; }

	[Serialize]
	public string SpriteName { get; init; }

	[Serialize("DescriptionLocKey")]
	public LocalizedText Description { get; init; }

	[Serialize("DescriptionShortLocKey")]
	public LocalizedText DescriptionShort { get; init; }

	[Serialize]
	private string DescriptionLocKey { get; init; }

	[Serialize]
	private string DescriptionShortLocKey { get; init; }
}
