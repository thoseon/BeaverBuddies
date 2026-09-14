using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;

namespace Timberborn.MortalSystem;

public record LethalNeedSpec : ComponentSpec
{
	[Serialize("DeathWarningLocKey")]
	public LocalizedText DeathWarning { get; init; }

	[Serialize("DeathMessageLocKey")]
	public LocalizedText DeathMessage { get; init; }

	[Serialize]
	private string DeathWarningLocKey { get; init; }

	[Serialize]
	private string DeathMessageLocKey { get; init; }
}
