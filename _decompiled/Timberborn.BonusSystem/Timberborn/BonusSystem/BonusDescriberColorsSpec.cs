using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BonusSystem;

internal record BonusDescriberColorsSpec : ComponentSpec
{
	[Serialize]
	public Color PositiveBonusHighlight { get; init; }

	[Serialize]
	public Color NegativeBonusHighlight { get; init; }
}
