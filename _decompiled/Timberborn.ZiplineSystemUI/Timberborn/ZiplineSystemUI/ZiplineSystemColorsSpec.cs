using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.ZiplineSystemUI;

internal record ZiplineSystemColorsSpec : ComponentSpec
{
	[Serialize]
	public Color OriginColor { get; init; }

	[Serialize]
	public Color ConnectableColor { get; init; }

	[Serialize]
	public Color NotConnectableColor { get; init; }
}
