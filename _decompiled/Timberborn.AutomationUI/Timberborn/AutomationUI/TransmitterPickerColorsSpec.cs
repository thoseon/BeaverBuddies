using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.AutomationUI;

internal record TransmitterPickerColorsSpec : ComponentSpec
{
	[Serialize]
	public Color TransmitterColor { get; init; }

	[Serialize]
	public Color UnfinishedTransmitterColor { get; init; }

	[Serialize]
	public Color HoveredTransmitterColor { get; init; }
}
