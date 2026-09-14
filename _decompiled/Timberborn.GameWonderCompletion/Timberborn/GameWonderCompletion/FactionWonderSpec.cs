using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;
using UnityEngine;

namespace Timberborn.GameWonderCompletion;

public record FactionWonderSpec : ComponentSpec
{
	[Serialize("WonderCompletionFlavorLocKey")]
	public LocalizedText WonderCompletionFlavor { get; init; }

	[Serialize("WonderCompletionMessageLocKey")]
	public LocalizedText WonderCompletionMessage { get; init; }

	[Serialize]
	public AssetRef<Sprite> WonderCompletionImage { get; init; }

	[Serialize]
	public string WonderLaunchSound { get; init; }

	[Serialize]
	private string WonderCompletionFlavorLocKey { get; init; }

	[Serialize]
	private string WonderCompletionMessageLocKey { get; init; }
}
